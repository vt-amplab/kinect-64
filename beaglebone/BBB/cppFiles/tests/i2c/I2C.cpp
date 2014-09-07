#include "I2C.h"

I2C::I2C(const int bus) {
	char* device = new char[50];
    sprintf(device, "/dev/i2c-%d", bus);

	if ((i2cfd = open(device, O_RDWR)) < 0) {
		sprintf(device, "Failed to open %s", device);
		perror(device);
	}
	delete[] device;
}

int I2C::startbus(const int address) {
	char* addr = new char[50];

	sprintf(addr, "0x%x", address);

	if (ioctl(i2cfd, I2C_SLAVE, address) < 0) {
		sprintf(addr, "Start error with address %s", addr);
        perror(addr);
        delete[] addr;
        return -1;
    }
    delete[] addr;

    return 0;
}

int I2C::readbus(char* data, const int bytes) {
	int tmp;

	tmp = read(i2cfd, data, bytes);

	if (tmp != bytes)
        perror("Read did not return bytes specified");

    return tmp;
}

int I2C::writebus(char* data) {
	return write(i2cfd, data, (sizeof(data)/sizeof(data[0])));
}

void I2C::closebus() {
	close(i2cfd);
}

I2C::~I2C() {
	closebus();
}