"Based on https://github.com/openai/baselines/blob/master/baselines/ppo1/pposgd_simple.py"

import tensorflow as tf
import numpy as np
from normal_distribution import NormalDistribution


class PPOModel:
    def __init__(self, num_states, num_actions=6 , hidden_size=52, num_hidden_layers = 2, epsilon_clip=0.2, gamma=0.99, lam=0.95):
        self.num_states = num_states
        self.num_actions = num_actions
        self.hidden_size = hidden_size
        self.num_hidden_layers = num_hidden_layers
        self.lose_rate = 1e-4
        self.var = 1.0
        self.epsilon_clip = epsilon_clip
        self.distribution = NormalDistribution()
        self.build_actor_and_critic()
        self.gamma = gamma
        self.lam = lam
    
    def ppo_loss_continuous(self, advantage, old_prediction):
        def loss(y_true, y_pred):
            denom = tf.keras.backend.sqrt(tf.keras.backend.variable(2.0 * np.pi * self.var))
            prob_num = tf.keras.backend.exp(- tf.keras.backend.square(y_true - y_pred) / (2 * self.var))
            old_prob_num = tf.keras.backend.exp(- tf.keras.backend.square(y_true - old_prediction) / (2 * self.var))

            prob = prob_num/denom
            old_prob = old_prob_num/denom
            
            r = prob/(old_prob + 1e-10)

            return -tf.keras.backend.mean(tf.keras.backend.minimum(r * advantage, tf.keras.backend.clip(r, min_value=1 - self.epsilon_clip, max_value=1 + self.epsilon_clip) * advantage))
        return loss

    
    def build_actor_and_critic(self):
        inputs = tf.keras.Input(shape=(self.num_states,))
        advantage = tf.keras.Input(shape=(1,))
        old_prediction = tf.keras.Input(shape=(self.num_actions,))
        
        x = tf.keras.layers.Dense(self.hidden_size, activation="relu")(inputs)
        for _ in range(self.num_hidden_layers - 1):
            x = tf.keras.layers.Dense(self.hidden_size, activation="relu")(x)
        
        out_actor = tf.keras.layers.Dense(self.num_actions, activation="tanh")(x)
        out_critic = tf.keras.layers.Dense(1)(x)

        self.actor = tf.keras.models.Model(inputs=[inputs], outputs=[out_actor])
        self.critic = tf.keras.models.Model(inputs=[inputs], outputs=[out_critic])

        self.actor.compile(
            optimizer=tf.keras.optimizers.Adam(lr=self.lose_rate),
            loss=[self.ppo_loss_continuous(
                advantage=advantage,
                old_prediction=old_prediction)]
            )
        #self.critic.compile(optimizer=tf.keras.optimizers.Adam(lr=self.lose_rate), loss="mse")
        
        self.actor.summary()
        self.critic.summary()

    def next_action_and_value(self, observ):
        self.distribution.mean = self.actor.predict(observ)
        return self.distribution.sample(), self.critic.predict(observ)
    

    def add_vtarg_and_adv(self, ep_dic):
        """
        Compute target value using TD(lambda) estimator, and advantage with GAE(lambda)
        """
        T = len(ep_dic["rewards"])
        ep_dic["values"] = np.append(ep_dic["values"], 0)
        ep_dic["adv"] = gaelam = np.empty(T, 'float32')
        lastgaelam = 0
        for t in range(T-1, -1, -1):
            delta = ep_dic["rewards"][t] + self.gamma * ep_dic["values"][t+1] - ep_dic["values"][t]
            gaelam[t] = lastgaelam = delta + self.gamma * self.lam * lastgaelam
        ep_dic["values"] = np.delete(ep_dic["values"], -1)
        ep_dic["tdlamret"] = ep_dic["adv"] + ep_dic["values"]

        
#ppo_model = PPOModel(10)
#ppo_model.build_actor_and_critic()
