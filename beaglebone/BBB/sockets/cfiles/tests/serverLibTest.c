#include "sockets.h"

#define dataSize 	1024
#define port 		4567

int main() {
	int* socketfd = malloc(sizeof(int));
	int* connectedSock = malloc(sizeof(int));
	int close, dataLen;
	char dataSend[dataSize], dataRecv[dataSize];

	if (serverInit(socketfd, port, dataSize)) {
		return -1;
	}
	printf("Server initialized\n");

	if (serverListen(socketfd, 5)) {
		return -1;
	}

	while (1) {
		printf("Server listening\n");
		connectedSock = serverAcceptConnection(socketfd);
		printf("Client at %s is connected on port %d\n", serverGetClientIP(), serverGetClientPort());
		serverSend(connectedSock, "You are connected\n");
		close = 0;

		while (*connectedSock != -1 && !close) {
			dataLen = serverRecieve(connectedSock, dataRecv);

			if(dataLen > 0 && dataRecv[dataLen -1] == '\n') {
				strcpy(dataSend, dataRecv);
				dataSend[dataLen] = '\0';
				dataRecv[dataLen - 1] = '\0';
				serverSend(connectedSock, dataSend);
				printf("Rx: %s", dataSend);

				if(strcmp(dataRecv, "exit") == 0) {
					usleep(10);
					serverSend(connectedSock, "You have been disconnected\n");
					serverCloseClient(connectedSock);
					printf("Client disconnected\n");
					free(connectedSock);
					close = 1;
				}
			}
		}
	}

	free(socketfd);

	return 0;
}