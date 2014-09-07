#include "chipCap2.h"

int* cc2Init(int addr, int bus) {
	int* i2cfd;

	i2cfd = I2COpenBus(bus);

	if (*i2cfd < 0) return (int*)-1;

	if (I2CStartSlave(i2cfd, addr) < 0) return (int*)-1;

	return i2cfd;
}

float cc2GetHumidity(int* i2cfd) {
	char data[5];
	float value;

	data[0] = 0xff;

	while ((data[0] >> 6) != 0) {
		if(I2CRead(i2cfd, 4, data) != 4) {
			perror("Read did not return bytes specified");
			return -1;
		} else if ((data[0] >> 6) == 0) {
			value = (((float)(data[0]&0x3f)*256 + data[1])/16384)*100;
		}
	}

	return value;
}

float cc2GetTemperature(int* i2cfd) {
	char data[5];
	float value;

	data[0] = 0xff;

	while ((data[0] >> 6) != 0) {
		if(I2CRead(i2cfd, 4, data) != 4) {
			perror("Read did not return bytes specified");
			return -1;
		} else if ((data[0] >> 6) == 0) {
			value = (data[2]*64 + (float)(data[3]>>2)/4)/16384*165-40;
		}
	}

	return value;
}

void cc2Close(int* i2cfd) {
	I2CCloseBus(i2cfd);
}