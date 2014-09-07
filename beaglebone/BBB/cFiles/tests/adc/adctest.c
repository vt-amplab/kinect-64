#include "adc.h"

int main() {
	int value;
	float voltage;

	while(1) {
		value = ADCReadValue(0);
		voltage = ADCReadVoltage(0, 1.8);

		printf("value: %d, voltage: %f\n", value, voltage);
	}

	return 0;
}