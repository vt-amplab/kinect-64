#ifndef UART_H
#define UART_H

#include <stdio.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <termios.h>
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <sys/ioctl.h>
#include <unistd.h>


#define _POSIX_SOURCE 1 /* POSIX compliant source */
#define FALSE 0
#define TRUE 1

class UART {
private:
	int uartfd;
public:
	UART(int bus);
	~UART();
	int startbus(int BUADRATE, int FLAGS);
	int readbus (char* data, const int bytes);
	int writebus(char* data);
	void closebus();




	/*int uartenable (int BAUDRATE, char* DEVICE, int FLAGS);//given baud rate, port and flags, returns fd
char* uartread (int fd, int bytes); //given fd, reads array of char from uart port and returns it
int uartwrite (int fd, char* data); //given fd and data, writes data to uart port
int uartclose (int fd); // given fd, closes uart port
*/

};

#endif