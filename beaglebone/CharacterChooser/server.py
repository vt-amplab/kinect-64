import socket

IP = ''
PORT = 9999
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

sock.bind((IP, PORT))
sock.listen(1)

conn, addr = sock.accept()
print addr

while True:
    data = conn.recv(24)
    conn.sendall(data)
