import socket, sys, os, json


import tensorflow as tf
import numpy as np

from ppo_model import PPOModel

class TrainModel:
    def __init__(self):
        self.server_adr = "./ai_controller"
        self.num_actions = 6
        # TODO: Dynamically set the number of states and actions
        self.ppo_model = PPOModel(num_states=2079, num_actions=self.num_actions)
        #self.ppo_model.build_actor_and_critic()

    def start(self):
        with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as s:
            s.bind(self.server_adr)
            s.listen(1)
            print("Starting accepting")
            while True:
                conn, client_adr = s.accept()
                print("Connected by", client_adr)
                ep_dic = self.run_episode(conn)
                self.ppo_model.add_vtarg_and_adv(ep_dic)
                print("tdlamret", ep_dic["tdlamret"])
                print("adv", ep_dic["adv"])
                # self.ppo_model.update_old_model()
                #print("LOSS: ", self.ppo_model.train(ep_dic, 0))
                self.ppo_model.train(ep_dic)
                return

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
        with conn:
            prev_state = None
            cur_pair_num = 0
            while True:
                # Read the packet header
                data_header = conn.recv(8)
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
                action, val = self.ppo_model.next_action_and_value(state_arr)
                action = action.numpy()[0]
                # print("action: ", action)
                # print("value:", val[0])
                val_preds.append(val[0])
                actions.append(action)
                means.append(self.ppo_model.distribution.mean)
                


                """
                # save pair state reward pair
                if prev_state:
                    with open("train_data/pair" + str(cur_pair_num) + ".json", "w") as f:
                        pair = {"state" : prev_state, "reward" : env_dic["reward"]}
                        json.dump(pair, f)
                        cur_pair_num += 1
                """

                if env_dic["done"]:
                    # Tell the game to restart the episode
                    conn.send("0".encode())
                    print("SEND LAST")
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
                    prev_state = env_dic["state"]
                    # Submit to model
                    #action = self.next_action()
                    action = self.format_action(action.tolist())
                    actionLenStr = str(len(action))
                    actionLenStr = (8 - len(actionLenStr)) * "0" + actionLenStr
                    conn.send(actionLenStr.encode())
                    conn.send(action.encode())

   
    def format_action(self, action):
        action_dic = {"vertical" : action[0], "horizontal" : action[1], "pitch" : action[2], "yaw" : action[3], "jump" : action[4], "attack" : action[5]}
        return json.dumps(action_dic)

    def next_action(self):
        temp_action = {"vertical" : 1, "horizontal" : 0, "pitch" : 0, "yaw" : 0, "jump" : 0, "attack" : 1}
        return json.dumps(temp_action)

if os.path.exists("ai_controller"):
    os.remove("ai_controller")

agent = TrainModel()
# agent.build_model()
agent.start()
