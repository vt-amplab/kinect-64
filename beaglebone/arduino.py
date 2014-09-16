import socket
import Adafruit_BBIO.UART as UART
import serial

UART.setup("UART1")
ser = serial.Serial(port='/dev/ttyO1', baudrate=9600)
ser.close()
ser.open()
ser.write('1N\n')
ser.write('2N\n')

# Create a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.bind(('', 8889))
sock.listen(1)

while True:
    print "here"
    conn, addr = sock.accept()
    print addr
    read = ''
    while True:
        try:
            read = conn.recv(2)
            print read
            break
        except Exception as e:
            print e
            break
            
    if (read == '1R') or (read == '1N') or (read == '2R') or (read == '2N') or (read == "1K") or (read == "2K") or (read == "1A") or (read == "2A"):
        ser.write(read + '\n')
	conn.close()
