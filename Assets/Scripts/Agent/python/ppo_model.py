import tensorflow as tf
import numpy as np


class PPOModel:
    def __init__(self, num_states, num_actions = 11, hidden_size=256, num_hidden_layers = 5, epsilon_clip=0.2 ):
        self.num_states = num_states
        self.num_actions = num_actions
        self.hidden_size = hidden_size
        self.num_hidden_layers = num_hidden_layers
        self.lose_rate = 1e-4
        self.var = 1.0
        self.epsilon_clip = epsilon_clip

    
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

        self.actor = tf.keras.models.Model(inputs=[inputs, advantage, old_prediction], outputs=[out_actor])
        self.critic = tf.keras.models.Model(inputs=[inputs], outputs=[out_critic])

        self.actor.compile(
            optimizer=tf.keras.optimizers.Adam(lr=self.lose_rate),
            loss=[self.ppo_loss_continuous(
                advantage=advantage,
                old_prediction=old_prediction)]
            )
        self.critic.compile(optimizer=tf.keras.optimizers.Adam(lr=self.lose_rate), loss="mse")
        
        self.actor.summary()
        self.critic.summary()
        
ppo_model = PPOModel(10)
ppo_model.build_actor_and_critic()
