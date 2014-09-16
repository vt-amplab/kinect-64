#include "sockets.h"

#define dataSize 	1024
#define port		4567
#define ip			"192.168.69.19"

int main() {
	int* socketfd = malloc(sizeof(int));
	int close, dataLen;
	char dataSend[dataSize], dataRecv[dataSize];

	if (clientInit(socketfd, ip, port, dataSize)) {
		return -1;
	}
	printf("Client initialized\n");

	if (clientConnect(socketfd)) {
		return -1;
	}
	close = 0;

	dataLen = clientRecieve(socketfd, dataRecv);
	dataRecv[dataLen - 1] = '\0';

	printf("Server: %s\n", dataRecv);

	while(!close) {
		printf("Tx: ");
		fgets(dataSend, 1024, stdin);
		clientSend(socketfd, dataSend);

		dataLen = clientRecieve(socketfd, dataRecv);
		dataRecv[dataLen - 1] = '\0';
		printf("Rx: %s\n", dataRecv);

		if (strcmp(dataSend, "exit\n") == 0) {
			dataLen = clientRecieve(socketfd, dataRecv);
			dataRecv[dataLen - 1] = '\0';
			close = 1;
			printf("%s\nService terminated by user\n", dataRecv);
		}
	}

	return 0;
}