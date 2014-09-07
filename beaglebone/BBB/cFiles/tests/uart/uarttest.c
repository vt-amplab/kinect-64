#include "uart.h"

#define BAUDRATE B19200

int main() {
	int* fd;
	int size, tmp;
	char buffer[255];

	fd = uartEnable(BAUDRATE, 4, CS8 | CLOCAL | CREAD);

	tmp = uartWrite(fd, "Connected\n");

	while(1) {
		printf("bytes: %d\n", tmp);
		size = uartRead(fd, 255, buffer);
		buffer[size-1] = '\0';

		printf("Rx: %s\n", buffer);

		tmp = uartWrite(fd, buffer);
	}
	uartClose(fd);

	return 0;
}