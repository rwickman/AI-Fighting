import socket, sys, os, json


import tensorflow as tf
import numpy as np

class AgentModel:
    def __init__(self):
        self.server_adr = "./ai_controller"
        self.num_actions = 6
        self.model = None

    def start(self):
        with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as s:
            s.bind(self.server_adr)
            s.listen(1)
            print("Starting accepting")
            while True:
                conn, client_adr = s.accept()
                with conn:
                    prev_state = None
                    cur_pair_num = 0
                    print("Connected by", client_adr)
                    while True:
                        # Read the packet header
                        data_header = conn.recv(8)
                        data_len = int(data_header.decode().rstrip("\x00"))
                        #print(data_len)
                        data = conn.recv(data_len, socket.MSG_WAITALL)
                        if not data:
                            break
                        env_dic = json.loads(data)
                        print(env_dic["reward"])
                        
                        # save pair state reward pair
                        if prev_state:
                            with open("train_data/pair" + str(cur_pair_num) + ".json", "w") as f:
                                pair = {"state" : prev_state, "reward" : env_dic["reward"]}
                                json.dump(pair, f)
                                cur_pair_num += 1



                        if env_dic["done"]:
                            # Tell the game to restart the episode
                            conn.send("0".encode())
                            print("SEND LAST")
                            conn.close()
                            break
                        else:
                            prev_state = env_dic["frame"]
                            # Submit to model
                            action = self.next_action()
                            actionLenStr = str(len(action))
                            actionLenStr = (8 - len(actionLenStr)) * "0" + actionLenStr
                            conn.send(actionLenStr.encode())
                            conn.send(action.encode())
   
    
    def build_model(self):
        # tanh -> [-1,1], sigmoid -> [0,1]
        vgg16_model = tf.keras.applications.vgg16.VGG16(include_top=True)
        vertical = tf.keras.layers.Dense(1, activation="tanh")(vgg16_model.layers[-2].output)
        horizontal = tf.keras.layers.Dense(1, activation="tanh")(vgg16_model.layers[-2].output)
        pitch = tf.keras.layers.Dense(1, activation="sigmoid")(vgg16_model.layers[-2].output)
        yaw = tf.keras.layers.Dense(1, activation="sigmoid")(vgg16_model.layers[-2].output)
        jump = tf.keras.layers.Dense(1, activation="sigmoid")(vgg16_model.layers[-2].output)
        attack = tf.keras.layers.Dense(1, activation="sigmoid")(vgg16_model.layers[-2].output)
        actions = tf.keras.layers.concatenate([vertical,horizontal,pitch,yaw,jump,attack])  
        #x = tf.keras.layers.Dense(self.num_actions, name='actions')(vgg16_model.layers[-2].output)
        self.model = tf.keras.models.Model(inputs=vgg16_model.input, outputs=actions)
        self.model.summary()

    def next_action(self):
        temp_action = {"vertical" : 1, "horizontal" : 0, "pitch" : 0, "yaw" : 0, "jump" : 0, "attack" : 1}
        return json.dumps(temp_action)

if os.path.exists("ai_controller"):
    os.remove("ai_controller")

agent = AgentModel()
agent.build_model()
#agent.start()
