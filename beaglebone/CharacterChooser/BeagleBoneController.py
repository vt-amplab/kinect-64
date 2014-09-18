import CharacterRecogniser as CR
import socket
import time
import select
from struct import *

# IP = '192.168.1.6'
IP = 'localhost'
PORT = 9999
# cr = CR.CharacterRecogniser()
stateFirst = True
state = 0
start = time.time()
mode = (0, 0)

while True:
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    while True:
        try:
            time.sleep(1)
            s.connect((IP, PORT))
            break
        except Exception as e:
            print "Connect: ", e

    while True:
        end = time.time()
        try:
            ready_read, ready_write, in_error = select.select([s,], [s,], [], 5)
        except select.error:
            s.close()
            s.shutdown(0)
            break

        if (len(ready_write) > 0) and ((end - start) >= 2):
            heart = pack('BBBB', 3, 0, 0, 0)
            s.send(heart)
            start = time.time()

        if len(ready_read) > 0:
            try:
                mode = s.recv(2)
                mode = unpack('BB', mode)
                print "Mode: ", mode[1]
            except Exception as e:
                print "Receive: ", e
                s.close()
                break

# STATE MACHINE
        if state == 1:
            if stateFirst:
                print "Setting Up"
                stateFirst = False
                i = 0
            # print "Run: ", i
            if state != mode[1]:
                print "Changing Mode!"
                state = mode[1]
                stateFirst = True

        elif state == 2:
            if stateFirst:
                print "Pick Character"
                stateFirst = False
                i = 0
                p1Choice = 1
                p2Choice = 2
                p1 = pack('BBBB', 0, p1Choice, 0, 0)
                p2 = pack('BBBB', 0, p2Choice, 0, 1)
                s.send(p1)
                time.sleep(1)
                s.send(p2)
                print "P1: ", p1Choice
                print "P2: ", p2Choice

            # print "Run: ", i
            if state != mode[1]:
                print "Changing Mode!"
                state = mode[1]
                stateFirst = True

        elif state == 3:
            if stateFirst:
                print "Pick Map"
                stateFirst = False
                i = 0
            # print "Run: ", i
            if state != mode[1]:
                print "Changing Mode!"
                state = mode[1]
                stateFirst = True

        elif state == 4:
            if stateFirst:
                print "Fighting"
                stateFirst = False
                i = 0
            # print "Run: ", i
            if state != mode[1]:
                print "Changing Mode!"
                state = mode[1]
                stateFirst = True

        elif state == 5:
            if stateFirst:
                print "Player 1 Wins"
                stateFirst = False
                i = 0
            # print "Run: ", i
            if state != mode[1]:
                print "Changing Mode!"
                state = mode[1]
                stateFirst = True

        elif state == 6:
            if stateFirst:
                print "Player 2 Wins"
                stateFirst = False
                i = 0
            # print "Run: ", i
            if state != mode[1]:
                print "Changing Mode!"
                state = mode[1]
                stateFirst = True

        elif state == 0:
            state = mode[1]



# while True:
#     recievedStart = False
#     while True:
#         try:
#             time.sleep(5)
#             s.connect((IP, PORT))
#             break
#         except Exception as e:
#             print e
#
#     while True:
#         heart = pack('BBBB', 3, 0, 0, 0)
#         s.send(heart)
#         time.sleep(1)
#         try:
#             start = s.recv(2)
#         except Exception as e:
#             print e
#             break
#         start = unpack('BB', start)
#
#         if start[1] == 2:
#             recievedStart = True
#             break
#         if start[1] == 5:
#             # cr.send_lights('2', 'K')
#             print 2
#         elif start[1] == 6:
#             # cr.send_lights('1', 'K')
#             print 1
#
#     if recievedStart:
#         # cr.send_lights('1', 'N')
#         # cr.send_lights('2', 'N')
#         # p1Choice, p2Choice = cr.choose()
#         p1Choice = 1
#         p2Choice = 2
#
#         print "P1: ", p1Choice
#         print "P2: ", p2Choice
#         p1 = pack('BBBB', 0, p1Choice, 0, 0)
#         p2 = pack('BBBB', 0, p2Choice, 0, 1)
#
#         # s.send(p1)
#         time.sleep(5)
#         s.send(p2)
#
#         s.close()