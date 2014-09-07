#include "ClientSock.h"

ClientSock::ClientSock(const char* ip, const int port) {
	struct hostent *host;

	this->ip = ip;
	this->port = port;

	host = gethostbyname(ip);

	socketfd = socket(AF_INET, SOCK_STREAM, 0);
	if (socketfd < 0) 
		perror("Socket");

	connected_server_addr.sin_family = AF_INET;
	connected_server_addr.sin_port = htons(port);
	connected_server_addr.sin_addr = *((struct in_addr*)host->h_addr);
	bzero(&(connected_server_addr.sin_zero), 8);
}

void ClientSock::reInit() {
	struct hostent *host;

	host = gethostbyname(ip);

	socketfd = socket(AF_INET, SOCK_STREAM, 0);
	if (socketfd < 0) 
		perror("Socket");

	connected_server_addr.sin_family = AF_INET;
	connected_server_addr.sin_port = htons(port);
	connected_server_addr.sin_addr = *((struct in_addr*)host->h_addr);
	bzero(&(connected_server_addr.sin_zero), 8);
}

int ClientSock::connectSock() {
	if (connect(socketfd, (struct sockaddr*)&connected_server_addr, sizeof(struct sockaddr)) < 0) {
		perror("Connect");
		return -1;
	}

	return socketfd;
}
	
int ClientSock::sendSock(const char* sendData) {
	int sent = send(socketfd, sendData, strlen(sendData), 0);
	
	if (sent < 0) {
		perror("Send data");
		return -1;
	}
	
	return sent;
}


int ClientSock::sendSock(const char* sendData, unsigned int datalength) {
	int sent = send(socketfd, sendData, datalength, 0);
	
	if (sent < 0) {
		perror("Send data");
		return -1;
	}
	
	return sent;
}

int ClientSock::readSock(char* recievedData, const int recieveLength) {
	int recieved = recv(socketfd, recievedData, recieveLength, 0);

	if (recieved < 0) {
		perror("Recieve data");
		return -1;
	}
	return recieved;
}

void ClientSock::closeSock() {
	close(socketfd);
}

ClientSock::~ClientSock() {
	closeSock();
}