#include "i2c.h"

int* I2COpenBus (int bus)
{	
    char* devicetmp = "/dev/i2c-", device[12], temp[25];
	int* i2cfd = malloc(sizeof(int));

    sprintf(device, "%s%d", devicetmp, bus);

	if ((*i2cfd = open(device, O_RDWR)) < 0) {
        sprintf(temp, "Failed to open %s\n", device);
        perror(temp);
        return (int*)-1;
    }
    
    return i2cfd;
}

int I2CStartSlave (int* i2cfd, int address) {
	if (ioctl(*i2cfd, I2C_SLAVE, address) < 0) {
        perror("Error with start");
        return -1;
    }
    
    return 0;

}

int I2CRead (int* i2cfd, int bytes, char* data) {
	if (read(*i2cfd, data, bytes) != bytes) {
        perror("Read did not return bytes specified");
        return -1;
    }

    return bytes;
}

int I2CWrite (int* i2cfd, char* data) {
	return write(*i2cfd, data, (sizeof(data)/sizeof(data[0])));
}

int I2CCloseBus (int* i2cfd) {
	return close(*i2cfd);
}