#include "uart.h"
/*H**********************************************************************
* FILENAME :        uart.c
*
* DESCRIPTION :
*       functions from uart.h for beaglebone black angstrum.
*
* PUBLIC FUNCTIONS :
*       int uartenable (int BAUDRATE, char* DEVICE, int FLAGS);
*       char* uartread (int fd, int bytes); 
*       int uartwrite (int fd, char* data); 
*       int uartclose (int fd); 
*
*
* AUTHOR :    William Gerhard        START DATE :    11 Jun 14
*
* CHANGES :
*
* REF NO  VERSION DATE    WHO     DETAIL
*
*
*H*/


/*
 * Function:  uartenable
 * --------------------
 *  Enables a uart port on beaglebone black at location given by DEVICE
 *
 *
 *  inputs:
 * BAUDRATE: the desired baudrate
 * uartNum: file location of the uart port beign enabled
 * FLAGS: basic configuration flags
 *
 *  returns: file handle for functions below
 */
int* uartEnable (int BAUDRATE, int uartNum, int FLAGS) {
	int* fd = malloc(sizeof(int));
	struct termios newtio;
	char* deviceTemp = "/dev/ttyO", device[10];

	sprintf(device, "%s%d", deviceTemp, uartNum);

	*fd = open(device, O_RDWR | O_NOCTTY);

	if (*fd < 0) {
		perror(device);
		return (int*)-1;
	}

	bzero(&newtio, sizeof(newtio)); /* clear struct for new port settings */

	/*  BAUDRATE:   Set bps rate. You could also use cfsetispeed and cfsetospeed.
		CRTSCTS :   output hardware flow control (only used if the cable has
					all necessary lines. See sect. 7 of Serial-HOWTO)
		CS8     :   8n1 (8bit,no parity,1 stopbit)
		CLOCAL  :   local connection, no modem contol
		CREAD   :   enable receiving characters */

	newtio.c_cflag = BAUDRATE | FLAGS;

	/*  IGNPAR  :   ignore bytes with parity errors
					otherwise make device raw (no other input processing) */

	newtio.c_iflag = IGNPAR;

	/*  Raw output  */

	newtio.c_oflag = 0;

	/*  ICANON  :   enable canonical input disable all echo functionality, and
					don't send signals to calling program */

	newtio.c_lflag = ICANON;

	/*  now clean the modem line and activate the settings for the port */

	tcflush(*fd, TCIFLUSH);
	tcsetattr(*fd, TCSANOW, &newtio);

	return fd;
}


/*
 * Function:  uartread
 * --------------------
 *  reads x bytes of data from uart port given in file handle
 *
 *
 *  inputs:
 * fd: file handel for enabled UART port
 * bytes: number of bytes you want to read it
 *
 *
 *  returns: pointer to array with read data
 */
int uartRead (int* fd, int bytes, char* data) {
	return read(*fd, data, bytes);
}


/*
 * Function:  uartwrite
 * --------------------
 *  writes out the char array passed to it to the file handle given
 *
 *
 *  inputs:
 * fd: file handle
 * data: data you want to write over serial
 *
 *
 *  returns: -1 if error, else 0
 */
int uartWrite (int* fd, char* data) {
	return write(*fd, data, (int)strlen(data));
}


/*
 * Function:  uartclose
 * --------------------
 *  closes uart port given via file handle
 *
 *
 *  inputs:
 * fd: file handle
 *
 *
 *
 *  returns: -1 if error, else 0
 */
int uartClose (int* fd) {
	return  close(*fd);
}