#include "sockets.h"

struct sockaddr_in server_addr, client_addr, connected_server_addr;
int recvDataSize;

/*************************/
/* Server side functions */
/*************************/
int serverInit(int* socketfd, const int port, const int recievedDataSize) {	
	const int true = 1;
	recvDataSize = recievedDataSize;

	*socketfd = socket(AF_INET, SOCK_STREAM, 0)
	if (*socketfd < 0) {
		perror("Socket");
		return -1;
	}

	if (setsockopt(*socketfd, SOL_SOCKET, SO_REUSEADDR, &true, sizeof(int)) < 0) {
		perror("Set socket options. SO_REUSEADDR");
		return -1;
	}

	server_addr.sin_family = AF_INET;
	server_addr.sin_port = htons(port);
	server_addr.sin_addr.s_addr = INADDR_ANY;
	bzero(&(server_addr.sin_zero), 8);

	if (bind(*socketfd, (struct sockaddr *)&server_addr, sizeof(struct sockaddr)) < 0) {
		perror("Unable to bind");
		return -1;
	}

	return 0;
}

int serverListen(const int* socketfd, const int connections) {
	if (listen(*socketfd, connections) < 0) {
		perror("Listen");
		return -1;
	}

	return 0;
}

int* serverAcceptConnection(const int* socketfd) {
	socklen_t sin_size = sizeof(client_addr);
	int* connectedSocketfd = malloc(sizeof(int));

	*connectedSocketfd = accept(*socketfd, (struct sockaddr *)&client_addr, &sin_size);
	
	if (*connectedSocketfd < 0) {
		perror("Accept");
		return (int*)-1;
	}

	return connectedSocketfd;
}

int serverSend(const int* connectedSocketfd, char* sendData) {
	int sent = send(*connectedSocketfd, sendData, strlen(sendData), 0);

	if (sent < 0) {
		perror("Send data");
		return -1;
	}
	
	return sent;
}

int serverRecieve(const int* connectedSocketfd, char* recievedData) {
	int recieved = recv(*connectedSocketfd, recievedData, recvDataSize, 0);
	
	if (recieved < 0) {
		perror("Recieve data");
		return -1;
	}

	return recieved;
}

void serverCloseClient(const int* connectedSocketfd) {
	close(*connectedSocketfd);
}

void serverClose(const int* socketfd) {
	close(*socketfd);
}

char* serverGetClientIP() {
	return inet_ntoa(client_addr.sin_addr);
}

int serverGetClientPort() {
	return ntohs(client_addr.sin_port);
}

/*************************/
/* Client side functions */
/*************************/
int clientInit(int* socketfd, const char* ip, const int port, const int recievedDataSize) {
	struct hostent *host;
	recvDataSize = recievedDataSize;

	host = gethostbyname(ip);

	*socketfd = socket(AF_INET, SOCK_STREAM, 0)
	if (*socketfd < 0) {
		perror("Socket");
		return -1;
	}

	connected_server_addr.sin_family = AF_INET;
	connected_server_addr.sin_port = htons(port);
	connected_server_addr.sin_addr = *((struct in_addr*)host->h_addr);
	bzero(&(connected_server_addr.sin_zero), 8);

	return 0;
}

int clientConnect(const int* socketfd) {	
	if (connect(*socketfd, (struct sockaddr*)&connected_server_addr, sizeof(struct sockaddr)) < 0) {
		perror("Connect");
		return -1;
	}

	return 0;
}

int clientSend(const int* socketfd, const char* sendData) {
	int sent = send(*socketfd, sendData, strlen(sendData), 0);
	
	if (sent < 0) {
		perror("Send data");
		return -1;
	}
	
	return sent;
}

int clientRecieve(const int* socketfd, char* revievedData) {
	int recieved = recv(*socketfd, revievedData, recvDataSize, 0);

	if (recieved < 0) {
		perror("Recieve data");
		return -1;
	}
	return recieved;
}

void clientClose(const int* socketfd) {
	close(*socketfd);
}