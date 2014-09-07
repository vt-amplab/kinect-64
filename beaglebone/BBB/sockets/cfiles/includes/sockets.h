#ifndef SOCKETS_H_
#define SOCKETS_H_

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

/*********************/
/* Server prototypes */
/*********************/
int serverInit(int* socketfd, const int port, const int recievedDataSize);
/* Returns 0 if initialized successfully and -1 otherwise. */

int serverListen(const int* socketfd, const int connections);
/* Returns 0 if set to listen correctly and -1 otherwise. */

int* serverAcceptConnection(const int* socketfd);
/* Returns connected socket file descriptor if accepted and */
/* -1 otherwise. This function is blocking. */

int serverSend(const int* connectedSocketfd, char* sendData);
/* Returns number of bytes sent and -1 otherwise. */

int serverRecieve(const int* connectedSocketfd, char* recievedData);
/* Returns number of bytes of data recieved from the client. */

void serverCloseClient(const int* connectedSocketfd);
/* Closes connection to the client. */

void serverClose(const int* socketfd);
/* Closes connection on the server. */

char* serverGetClientIP();
/* Returns character array of the connected clients IP. */

int serverGetClientPort();
/* Returns port number of the connected client. */

/*********************/
/* Client Prototypes */
/*********************/
int clientInit(int* socketfd, const char* ip, const int port, const int recievedDataSize);
/* Returns 0 if initialized successfully and -1 otherwise. */

int clientConnect(const int* socketfd);
/* Returns 0 if client connected to the server and -1 otherwise. */

int clientSend(const int* socketfd, const char* sendData);
/* Returns number of bytes that were sent to the server. If -1 is */
/* returned, then there has been an error. */

int clientRecieve(const int* socketfd, char* revievedData);
/* Returns number of bytes recieved. If connection was closed, then */
/* 0 will be returned. -1 is returned if there has been an error. */

void clientClose(const int* socketfd);
/* Closes connection to the server. */

#endif