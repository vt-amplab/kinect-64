#include "ADC.h"

int main() {

	ADC adc(0);
	int value;
	float voltage;

	while(1) {
		value = adc.ReadValue();
		voltage = adc.ReadVoltage(1.8);

		printf("value: %d, voltage: %f\n", value, voltage);
	}

	return 0;
}