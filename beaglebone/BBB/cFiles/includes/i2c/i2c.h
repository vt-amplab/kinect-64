#ifndef I2C_H_
#define I2C_H_

#include <linux/i2c-dev.h>
#include <stdio.h>
#include <stdlib.h>
#include <fcntl.h>
#include <sys/ioctl.h>
#include <unistd.h>
#include <string.h>


/*H**********************************************************************
* FILENAME :        i2c.h             
*
* DESCRIPTION :
*       Headerfiles for i2c functions for beaglebone black angstrum. 
*
* PUBLIC FUNCTIONS :
*       
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

int* I2COpenBus (int bus);
int I2CStartSlave (int* i2cfd, int address);
int I2CRead (int* i2cfd, int bytes, char* data);
int I2CWrite (int* i2cfd, char* data);
int I2CCloseBus (int* i2cfd);

#endif