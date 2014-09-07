#include "I2C.h"
#include <iostream>

using namespace std;

int main() {
	I2C i2c(2);
	char data[4];

	i2c.startbus(0x28);

	while(1) {
		i2c.readbus(data, 4);

		cout << "Humidity: " << (((float)(data[0]&0x3f)*256 + data[1])/16384)*100;
		cout << ", Temperature: " <<  (data[2]*64 + (float)(data[3]>>2)/4)/16384*165-40 << endl;
	}

	return 0;
}