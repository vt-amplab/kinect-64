#include "RTC.h"
#include <iostream>

using namespace std;

int main() {
	RTC rtc(1,0x68);

	while(1) {

		cout << "Time: " << rtc.getTime() << endl;
	}

	return 0;
}