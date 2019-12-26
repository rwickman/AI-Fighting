import socket, sys, os, json


class Agent:
    def __init__(self):
        self.server_adr = "./ai_controller"

    def start(self):
        with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as s:
            s.bind(self.server_adr)
            s.listen(1)
            print("Starting accepting")
            while True:
                conn, client_adr = s.accept()
                with conn:
                    print("Connected by", client_adr)
                    while True:
                        # Read the packet header
                        data_header = conn.recv(8)
                        data_len = int(data_header.decode().rstrip("\x00"))
                        print(data_len)
                        data = conn.recv(data_len, socket.MSG_WAITALL)
                        if not data:
                            break
                        json_str = json.loads(data)
                        # Submit to model
                        action = self.next_action()
                        actionLenStr = str(len(action))
                        actionLenStr = (8 - len(actionLenStr)) * "0" + actionLenStr
                        conn.send(actionLenStr.encode())
                        conn.send(action.encode())
    
    def next_action(self):
        temp_action = {"vertical" : 1, "horizontal" : 0, "pitch" : 0, "yaw" : 0, "jump" : 1, "attack" : 1}
        return json.dumps(temp_action)

agent = Agent()
agent.start()
