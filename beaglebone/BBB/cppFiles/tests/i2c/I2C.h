#ifndef I2C_H
#define I2C_H

#include <linux/i2c-dev.h>
#include <stdio.h>
#include <fcntl.h>
#include <sys/ioctl.h>
#include <unistd.h>

class I2C {
private:
	int i2cfd;
public:
	//!Constructor for the I2C class, takes one arguement and returns nothing
	/*!	When called, this will create a I2c class for the specified bus
	\param bus is the I2C bus being used (either 0, 1, or 2)
	\return none
	 */
	I2C(const int bus);

	//!Deconstructor for the I2C class
	/*!	When called, this closes control of the I2C bus
	\param gnone
	\return none
	 */
	~I2C();

	//!Initalizes the device at the given address on the bus declared in the constructor
	/*!	When called, this will write to the given address and initalize the device for future reads and writes
	\param address is the I2C device address
	\return the file descriptor if failed, otherwise 
	 */
	int startbus(const int address);

	//!Reads data from the device specified in the startbus
	/*!	When called, this will read data sent from the device given in the start bus function and return the number of bytes read and modify a char array with the data 
	\param data is the data location which the function will modify
	\param bytes is the number of bytes to be read
	\return the number of bytes read
	 */
	int readbus(char* data, const int bytes);

	//!Writes the data given to the I2C bus
	/*!	When called, this will write the data bytes given to the device on the bus
	\param data is the data location to be written to the device on the bus
	\return the number of bytes written
	 */

	int writebus(char* data);

	//!Releases control of the bus and the device given
	/*!	When called, this will release the specific device ID

	\return none
	 */
	void closebus();
};

#endif