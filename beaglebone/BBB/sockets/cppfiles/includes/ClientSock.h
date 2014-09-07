#ifndef CLIENTSOCK_H
#define CLIENTSOCK_H

#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <stdio.h>
#include <stdlib.h>
#include <unistd.h>
#include <errno.h>
#include <string.h>
#include <netdb.h>

class ClientSock {
private:
	int socketfd, port;
	const char* ip;
	struct sockaddr_in connected_server_addr;
public:
	ClientSock(const char* ip, const int port);
	~ClientSock();
	void reInit();
	int connectSock();
		int sendSock(const char* sendData);
	int sendSock(const char* sendData, unsigned int datalength);
	int readSock(char* recievedData, const int recieveLength);
	void closeSock();
};

#endif