import CharacterRecogniser as CR
import socket
import time
from struct import *

IP = '192.168.1.6'
PORT = 9999
cr = CR.CharacterRecogniser()
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

while True:
    while True:
        try:
            time.sleep(5)
            s.connect((IP, PORT))
            break
        except Exception as e:
            print e

    while True:
        heart = pack('BBBB', 3, 0, 0, 0)
        s.send(heart)
        time.sleep(1)
        start = s.recv(2)
        start = unpack('BB', start)
        if start[1] == 2:
            break

    p1Choice, p2Choice = cr.choose()

    print "P1: ", p1Choice
    print "P2: ", p2Choice
    p1 = pack('BBBB', 0, p1Choice, 0, 0)
    p2 = pack('BBBB', 0, p2Choice, 0, 1)

    s.send(p1)
    time.sleep(1)
    s.send(p2)

    s.close()