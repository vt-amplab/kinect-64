import CharacterRecogniser as CR
import socket

cr = CR.CharacterRecogniser()
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.bind(())

while True:
    p1Choice, p2Choice = cr.choose();


    print "P1: ", p1Choice
    print "P2: ", p2Choice