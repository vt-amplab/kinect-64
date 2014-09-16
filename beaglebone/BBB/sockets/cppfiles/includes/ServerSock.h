/****************************/
/* File: ServerSock.h       */
/* Creator: Jacob Yacovelli */
/* Date: 6/24/2014          */
/****************************/

#ifndef SERVERSOCK_H
#define SERVERSOCK_H

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

class ServerSock {
private:
	int socketfd; //!< Brief socketfd: file descriptor for the litening socket.
	fd_set master; //!< Brief master: file descriptor set to monitor the connected sockets.
	int fdmax; //!< Brief fdmax: int to hold largest file descriptor in set.
	struct sockaddr_in client_addr; //!< Brief client_addr: struct to hold network info on client.
public:	
	/******************************/
	/* ServerSock(const int port) */
	/******************************/
	//!Constructor for the ServerSock class, takes one 
	//!arguement and returns nothing
	/*! When called, this will bind a socket at the specified 
		port with a reusable address using TCP.	
	\param const int port - Port number associate with the socket
	\return None
	*/
	ServerSock(const int port);

	/*****************/
	/* ~ServerSock() */
	/*****************/
	//!Deconstructor for the ServerSock class
	/*! When called, this will close the listening port and all 
		connected clients.	
	\param None
	\return None
	*/
	~ServerSock();

	/*****************************************/
	/* int listenSock(const int connections) */
	/*****************************************/
	//!Sets bound socket from constructor to listen
	/*! When called, this will set the bound socket from the 
		constructor to listen. It will also add the listening 
		socket file descriptor to the master file descriptor set.
	\param const int connections - Max number of clients that can 
		connect
	\return int - Listening socket file descriptor, -1 if 
		there is an error
	*/
	int listenSock(const int connections);

	/********************/
	/* int acceptSock() */
	/********************/
	//!Accepts connection request
	/*!	When called, this will accept a connect request from the 
		listener and return the clients file descriptor. It will 
		also add the connected client file descriptor to the 
		master file descriptor set.
	\param None
	\return int - Connected client file descriptor, -1 if 
		there is an error
	*/
	int acceptSock();

	/****************************************************************/
	/* int sendSock(const int clientSocketfd, const char* sendData) */
	/****************************************************************/
	//!Sends data over given socket
	/*! When called, this will send sendData to the client 
		specified by clientSocketfd and return the byte 
		count that was sent.
	\param const int clientSocketfd - Client you wish to send data too
	\param const char* sendData - Data you wish to send
	\return int - Number of bytes sent to the client, -1 if there 
		is an error
	*/
	int sendSock(const int clientSocketfd, const char* sendData, unsigned int datalength);


	int sendSock(const int clientSocketfd, const char* sendData);


	/*********************************************************************************************/
	/* int readSock(const int clientSocketfd, const char* recievedData, const int recieveLength) */
	/*********************************************************************************************/
	//!Recieves data sent over sockets
	/*!	When called, this will store recieved data, up 
		to the byte length given from recieveLength, from 
		clientSocketfd into recievedData. It will return 
		the length of bytes recieved.
	\param const int clientSocketfd - Client you wish to recieve 
		the data from
	\param const char* recievedData -Buffer to store the data
	\param const int recieveLength - Maximum bytes to read in
	\return int - Number of bytes recieved from the client -1 if 
		there is an error
	*/
	int readSock(const int clientSocketfd, char* recievedData, const int recieveLength);

	/**********************************************/
	/* void closeClient(const int clientSocketfd) */
	/**********************************************/
	//!Closes socket
	/*! When called, this will close the connection to 
		clientSocketfd and remove clientSocketfd from the 
		master file descriptor set.
	\param const int clientSocketfd - Client you wish to close connection to
	\return None
	*/
	void closeClient(const int clientSocketfd);

	/**************************/
	/* void closeAllClients() */
	/**************************/
	//!Closes all connections to server
	/*! When called, this will close all the connections to 
		the server and remove them from the master file 
		descriptor set.
	\param None
	\return None
	*/
	void closeAllClients();

	/**********************/
	/* void closeServer() */
	/**********************/
	//!Closes the listening socket
	/*! When called, this will close the listening socket
	\param None
	\return None
	*/
	void closeServer();

	/***********************/
	/* int getClientPort() */
	/***********************/
	//!Gets port number of last client
	/*! When called, this will return the port number of 
		the last connected client.
	\param None
	\return int - Last connected clients port number
	*/
	int getClientPort();

	/***********************/
	/* char* getClientIP() */
	/***********************/
	//!Gets IP address of last client
	/*! When called, this will return the IP address 
		of the last connected client.
	\param None
	\return char* - Last connected clients IP address
	 */
	char* getClientIP();

	/*************************/
	/* fd_set* getMasterfd() */
	/*************************/
	//!Gets master file descriptor set
	/*! When called, this will return master file descriptor 
		set holding the listener and connected client file 
		descriptors.
	\param None
	\return fd_set* - File descriptors in the set
	*/
	fd_set* getMasterfd();

	/******************/
	/* int getMaxfd() */
	/******************/
	//!Gets largest file descriptor in the master file descriptor set
	/*! When called, this will return the largest file 
		descriptor in the master file descriptor set.
	\param None
	\return int - Largest file descriptor in the set
	*/
	int getMaxfd();
};

#endif