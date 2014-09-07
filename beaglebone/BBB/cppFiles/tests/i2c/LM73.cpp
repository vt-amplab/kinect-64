#include "LM73.h"
/*
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
*/



#define BUS 2

LM73::LM73(const int addr) : i2c(BUS) {
        i2c.startbus(addr);
}

LM73::LM73(const int bus, const int addr) : i2c(bus) {
        i2c.startbus(addr);
}

float LM73::getTemperature() {
        char* data = new char[5];
        float value;

        data[0] = 0xff;

        i2c.writebus(0x00);

        while ((data[0] >> 6) != 0) {
                if(i2c.readbus(data, 2) != 2) {
                        perror("Read did not return bytes specified");
                        delete[] data;
                        return -1;
                } else if ((data[0] >> 6) == 0) {
                        value = ((data[0] << 8) + data [1])/128.0;
        }
    }
        delete[] data;

        return value;
} 

void LM73::close() {
        i2c.closebus();
}

LM73::~LM73() {
        close();
}