#include "UART.h"
#include <iostream>

using namespace std;

#define BAUDRATE B19200

#define CHARBUF 256

int size, tmp;
char buffer[255];

char* str = new char[CHARBUF];

int main() {
	UART uart(4);


	uart.startbus(BAUDRATE, CS8 |CLOCAL | CREAD);
	uart.writebus("Connected\n");

	while(1) {

		printf("\n\nPlease Type Command:\n");
			fgets(str, CHARBUF, stdin);

			 tmp = uart.writebus(str);
			    printf("Tx: %s\n", str);


		printf("bytes: %d\n", tmp);
                size = uart.readbus(buffer, 255);
                buffer[size-1] = '\0';

                printf("Rx: %s\n", buffer);

                


		
	}

	return 0;
}