import socket, sys, os

server_adr = "./ai_controller2"

with socket.socket(socket.AF_UNIX, socket.SOCK_STREAM) as s:
    s.bind(server_adr)
    s.listen(1)
    print("Starting accepting")
    conn, client_adr = s.accept()
    with conn:
        print("Connected by", client_adr)
        while True:
            data = conn.recv(1024)
            if not data:
                break
            print(data)
