#include "i2c.h"

int main() {
	int* i2cfd;
	float humidity, temperature;
	char data[256];

	i2cfd = I2COpenBus(2);

	if(*i2cfd < 0)
		exit(1);

	if(I2CStartSlave(i2cfd, 0x28) < 0) {
		exit(1);
	}

	while(1) {
		if(I2CRead(i2cfd, 4, data) != 4) {
			perror("Read did not return bytes specified");
			exit(1);
		} else if ((data[0] >> 6) == 0) {
			humidity = (((float)(data[0]&0x3f)*256 + data[1])/16384)*100;
			temperature = (data[2]*64 + (float)(data[3]>>2)/4)/16384*165-40;
			printf("Humidity: %f\nTemperature: %f\n", humidity, temperature);
		}
	}
}