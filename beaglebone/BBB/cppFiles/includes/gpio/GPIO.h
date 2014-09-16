#ifndef GPIO_H_   /* Include guard */
#define GPIO_H_

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>
#include <unistd.h>
#include <fcntl.h>
#include <poll.h>

 /****************************************************************
 * Constants
 ****************************************************************/
 
#define SYSFS_GPIO_DIR "/sys/class/gpio"
#define POLL_TIMEOUT (3 * 1000) /* 3 seconds */
#define MAX_BUF 64

class GPIO {
private:
	
	unsigned int gpio;

public:
	//!Public file descriptor for use with select and polling
	/*!	Public file descriptor which can be used with select and polling without affect other functions in class
	\sa gpio_fd_open ()
	 */
	int GPIOfd; 

	//!Constructor for the GPIO class, takes one arguement and returns nothing
	/*!	When called, this will set the physical GPIO pin to the GPIO class
	\param gpio1 is the GPIO pin number, NOT the header pin number
	\return none
	 */
	GPIO(unsigned int gpio1);

	//!Deconstructor for the GPIO class
	/*!	When called, this remove all control of the GPIO pin
	\param gnone
	\return none
	 */
	~GPIO();

	//!enables control of GPIO pin
	/*!	When called, this will create a file descriptor for the GPIO pin, must be called before use
	\param none
	\return none
	 */
	int gpio_export();

/****************************************************************
 * gpio_unexport
 ****************************************************************/

 	//!Closes the file descriptor for the GPIO
	/*!	When called, this will close file descriptor the physical GPIO pin in the GPIO class
	\param none
	\return none
	 */
int gpio_unexport();

/****************************************************************
 * gpio_set_dir
 ****************************************************************/
  	//!Sets the pin and input or output
	/*!	Sets the direction for the GPIO pin, input or output
	\param out_flag sets direction, 0 is input 1 is output
	\return fd on error, 0 if no error
	 */
int gpio_set_dir(unsigned int out_flag);

/****************************************************************
 * gpio_set_value
 ****************************************************************/
   	//!Sets the pin high or low
	/*!	Sets the value for a pin set as output (low or high)
	\param value sets direction, 1 is high, 0 is low
	\return fd on error, 0 if no error
	 */
int gpio_set_value(unsigned int value);


/****************************************************************
 * gpio_get_value
 ****************************************************************/

   	//!Reads value of pin set as input
	/*!	Reads the vlaue of a pin set as an input
	\param *value is a pointer value changed outside of class
	\return fd on error, 0 if no error
	 */
int gpio_get_value(unsigned int *value);

/****************************************************************
 * gpio_set_edge
 ****************************************************************/

   	//!Sets edge detection for pin
	/*!	Sets the edge detection for pins configured as input, can be rising edge or fallign edge
	\param *edge sets the edge detection, 1 for rising, 0 for falling (needs to be double checked)
	\return fd on error, 0 if no error
	 */

int gpio_set_edge(char *edge);

/****************************************************************
 * gpio_fd_open
 ****************************************************************/

   	//!Creates seperate file descriptor, can be used for polling
	/*!	Creates a seperate file descriptor for the GPIO pin, allows use fo polling and select with affecting other functions
	\param none
	\return fd on error, 0 if no error
	 */

int gpio_fd_open();

/****************************************************************
 * gpio_fd_close
 ****************************************************************/

    //!Closes seperate file descriptor, can be used for polling
	/*!	Closes a seperate file descriptor for the GPIO pin
	\param none
	\return fd on error, 0 if no error
	 */


int gpio_fd_close();


};


#endif
