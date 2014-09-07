/****************************/
/* File: ServerSock.cpp     */
/* Creator: Jacob Yacovelli */
/* Date: 6/24/2014          */
/****************************/

#include "ServerSock.h"

ServerSock::ServerSock(const int port) : socketfd(),  master(), fdmax(), client_addr() {
	const int optval = 1;
	struct sockaddr_in server_addr;

	FD_ZERO(&master);

	socketfd = socket(AF_INET, SOCK_STREAM, 0);
	if (socketfd < 0)
		perror("Socket");

	if (setsockopt(socketfd, SOL_SOCKET, SO_REUSEADDR, &optval, sizeof(int)) < 0)
		perror("Set socket options. SO_REUSEADDR");

	server_addr.sin_family = AF_INET;
	server_addr.sin_port = htons(port);
	server_addr.sin_addr.s_addr = INADDR_ANY;
	bzero(&(server_addr.sin_zero), 8);

	if (bind(socketfd, (struct sockaddr *)&server_addr, sizeof(struct sockaddr)) < 0)
		perror("Unable to bind");
}

int ServerSock::listenSock(const int connections) {
	if (listen(socketfd, connections) < 0) {
		perror("Listen");
		return -1;
	}

	FD_SET(socketfd, &master);
	fdmax = socketfd;

	return socketfd;
}
	
int ServerSock::acceptSock() {
	socklen_t sin_size = sizeof(client_addr);
	int clientSocketfd;

	clientSocketfd = accept(socketfd, (struct sockaddr *)&client_addr, &sin_size);
	
	if (clientSocketfd < 0) {
		perror("Accept");
		return -1;
	}

	FD_SET(clientSocketfd, &master);
	if (clientSocketfd > fdmax)
		fdmax = clientSocketfd;
        
	return clientSocketfd;
}
	
int ServerSock::sendSock(const int clientSocketfd, const char* sendData) {
	int sent = send(clientSocketfd, sendData, strlen(sendData), 0);

	if (sent < 0) {
		perror("Send data");
		return -1;
	}
	
	return sent;
}


int ServerSock::sendSock(const int clientSocketfd, const char* sendData, unsigned int datalength) {
	int sent = send(clientSocketfd, sendData, datalength, 0);

	if (sent < 0) {
		perror("Send data");
		return -1;
	}
	
	return sent;
}
	
int ServerSock::readSock(const int clientSocketfd, char* recievedData, const int recieveLength) {
	int recieved = recv(clientSocketfd, recievedData, recieveLength, 0);
	
	if (recieved < 0) {
		perror("Recieve data");
		return -1;
	}

	return recieved;
}

void ServerSock::closeClient(const int clientSocketfd) {
	close(clientSocketfd);
	FD_CLR(clientSocketfd, &master);
}

void ServerSock::closeAllClients() {
	for (int i=0; i<= fdmax; i++) {
		if (FD_ISSET(i, &master) && (i != socketfd))
			closeClient(i);
	}
}
	
void ServerSock::closeServer() {
	close(socketfd);
}
	
int ServerSock::getClientPort() {
	return ntohs(client_addr.sin_port);
}
	
char* ServerSock::getClientIP() {
	return inet_ntoa(client_addr.sin_addr);
}

fd_set* ServerSock::getMasterfd() {
	return &master;
}

int ServerSock::getMaxfd() {
	return fdmax;
}

ServerSock::~ServerSock() {
	closeAllClients();
	closeServer();
}