import socket, threading, json
import tensorflow as tf
import numpy as np
from normal_distribution import NormalDistribution

class AgentWorker:
    def __init__(self, ppo_model):
        self.ppo_model = ppo_model
        self.local_actor = tf.keras.models.clone_model(self.ppo_model.actor)
        self.local_critic = tf.keras.models.clone_model(self.ppo_model.critic)
        self.local_actor.set_weights(self.ppo_model.actor.get_weights())
        self.local_critic.set_weights(self.ppo_model.critic.get_weights())
        self.header_len = 8
        self.distribution = NormalDistribution()

    def start_training_agent(self, conn):
        with conn:
            while True:
                ep_dic = self.run_episode(conn)
                self.ppo_model.add_vtarg_and_adv(ep_dic)
                # print("ADVANTAGE: ", ep_dic["adv"])
                # print("TDLAMRET", ep_dic["tdlamret"])
                print("REWARDS: ", ep_dic["rewards"])
                # self.ppo_model.update_old_model()
                #print("LOSS: ", self.ppo_model.train(ep_dic, 0))
                global_actor_weights, global_critic_weights = self.ppo_model.train(ep_dic)
                self.local_actor.set_weights(global_actor_weights)
                self.local_critic.set_weights(global_critic_weights)

                print("Done Training")
                end_msg = "0" * 3
                endLenStr = str(len(end_msg))
                endLenStr = (self.header_len - len(endLenStr)) * "0" + endLenStr
                conn.send(endLenStr.encode())
                conn.send(end_msg.encode())
                print("Sent Last")


    def run_episode(self, conn):
        # TODO: Tell the game to start a new episode
        observs = []
        rewards = []
        val_preds = []
        actions = []
        means = []
        #prev_acts = []
        ep_ret = 0
        ep_len = 0
        while True:
            # Read the packet header
            data_header = conn.recv(self.header_len)
            data_len = int(data_header.decode().rstrip("\x00"))
            #print(data_len)
            data = conn.recv(data_len, socket.MSG_WAITALL)
            
            if not data:
                break
            
            env_dic = json.loads(data)

            state_arr = np.array(env_dic["state"])
            state_arr = np.reshape(state_arr, (1, state_arr.shape[0]))
            observs.append(state_arr)
            rewards.append(env_dic["reward"])
            ep_ret += env_dic["reward"]
            ep_len += 1
            
            #print(env_dic["reward"])
            action, val = self.next_action_and_value(state_arr)
            action = action.numpy()[0]
            # print("action: ", action)
            # print("value:", val[0])
            val_preds.append(val[0])
            actions.append(action)
            means.append(self.distribution.mean)

            if env_dic["done"]:
                # Tell the game to restart the episode
                # conn.close()
                return {
                    "observations" : observs,
                    "rewards" : rewards,
                    "values" : val_preds,
                    "actions" : actions,
                    "means" : means,
                    "episode_return" : ep_ret,
                    "episode_length" : ep_len,
                    }
            else:
                # Send next action
                action = self.format_action(action.tolist())
                actionLenStr = str(len(action))
                actionLenStr = (self.header_len - len(actionLenStr)) * "0" + actionLenStr
                conn.send(actionLenStr.encode())
                conn.send(action.encode())

    def next_action_and_value(self, observ):
        self.distribution.mean = self.local_actor(observ)
        return self.distribution.sample(), self.local_critic(observ)
   
    def format_action(self, action):
        action_dic = {
            "vertical" : action[0],
            "horizontal" : action[1],
            "pitch" : action[2],
            "yaw" : action[3],
            "jump" : action[4],
            "attack" : action[5]
        }
        return json.dumps(action_dic)
        