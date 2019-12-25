import socket, sys, os, json

server_adr = "./ai_controller"
with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as s:
    s.bind(server_adr)
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
                print(str(data))
                json_str = json.loads(data)
                #print(data)
                conn.send("0".encode())
