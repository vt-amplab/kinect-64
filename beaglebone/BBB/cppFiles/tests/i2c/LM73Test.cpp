#include "LM73.h"
#include <iostream>

using namespace std;

int main() {
	LM73 lm73(1, 0x49);

	while(1) {

		cout << "Temperature: " << lm73.getTemperature() << endl;
	}

	return 0;
}
