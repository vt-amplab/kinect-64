#ifndef UART_H_   /* Include guard */
#define UART_H_


/*H**********************************************************************
* FILENAME :        uart.h             
*
* DESCRIPTION :
*       Headerfiles for UART functions for beaglebone black angstrum. 
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
#include <stdio.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>
#include <termios.h>
#include <string.h>
#include <stdlib.h>
#include <unistd.h>



//#define _POSIX_SOURCE 1 /* POSIX compliant source */

//prototypes functions

int* uartEnable (int BAUDRATE, int uartNum, int FLAGS);//given baud rate, port and flags, returns fd
int uartRead (int* fd, int bytes, char* data); //given fd, reads array of char from uart port and returns it
int uartWrite (int* fd, char* data); //given fd and data, writes data to uart port
int uartClose (int* fd); // given fd, closes uart port

#endif