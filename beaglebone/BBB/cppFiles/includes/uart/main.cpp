#include "UART.h"
#include <string>

using std::string;

int main() {
	UART uart(1);

	uart.startbus(9600, CS8 | CLOCAL | CREAD);

//	while (1) {
		string hi = "hellioo";

		uart.writebus((char*)hi.c_str());
//	}

	return 0;
}
