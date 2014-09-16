#include "ClientSock.h"
#include <stdio.h>
#include <stdlib.h>
#include <iostream>

#define CHARBUF 256

using std::cin;

struct __attribute__((packed)) ETH_BASE_MSG
{
  int8_t  mode;  
  int64_t control_options;
  float   settings;  
};

int main(int argc, char **argv) {
	int sockfd, strLen;
	char* str = new char[CHARBUF];
	struct ETH_BASE_MSG eth;
	char* ethChar = new char[sizeof(eth)];

	if (argc < 3) {
		printf("Too few arguments!\n");
        printf("Try: clientLibTest <ip> <port>\n");
        return -1;
	}

	ClientSock sock(argv[1], atoi(argv[2]));

	eth.mode = 0;
	eth.control_options = 12;
	eth.settings = 3.14;

	//char* struc = new char[sizeof(struct ETH_BASE_MSG) + 1];

	//sprintf(struc, "%s\0", (char*)eth);

	sockfd = sock.connectSock();
	if (sockfd < 0)
		return -1;

	while(1) {
		strLen = sock.readSock(str, CHARBUF);

		if(strLen <= 0) {
			printf("Server disconnected!\n");
			sock.closeSock();
			sock.reInit();
			sockfd = sock.connectSock();
						
			if (sockfd < 0)
				return -1;
		} else {
			str[strLen-1] = '\0';
			printf("Rx: %s\n", str);
		}

		memset(ethChar, 0, sizeof(eth));
		//ethChar = (char*)eth;
		//sock.sendSock(ethChar);
	}

	return 0;
}
