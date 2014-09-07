#include "lm73.h"

int* lm73Init(int addr, int bus) {
	int* i2cfd;

	i2cfd = I2COpenBus(bus);

	if (*i2cfd < 0) {
		perror("Cannot open bus");
		return (int*)-1;
	}
	if (I2CStartSlave(i2cfd, addr) < 0) {
		perror("Cannot send start");
		return (int*)-1;
	}

	return i2cfd;
}

float lm73GetTemperature(int* i2cfd) {
	char data[5];
	float value;

	data[0] = 0xff;

	I2CWrite(i2cfd, 0x00);

	if(I2CRead(i2cfd, 2, data) != 2) {
		perror("Read did not return bytes specified");
		return -1;
	} else {
		value = ((data[0] << 8) + data [1])/128.0;
	}

	return value;
}

void lm73Close(int* i2cfd) {
	I2CCloseBus(i2cfd);
}