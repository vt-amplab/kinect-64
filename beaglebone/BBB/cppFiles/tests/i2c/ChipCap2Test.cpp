#include "ChipCap2.h"
#include <iostream>

using namespace std;

int main() {
	ChipCap2 cc2(0x28);

	while(1) {
		cout << "Humidity: " << cc2.getHumidity();
		cout << ", Temperature: " << cc2.getTemperature() << endl;
	}

	return 0;
}