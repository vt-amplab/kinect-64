/****************************/
/* File: serverLibTest.cpp  */
/* Creator: Jacob Yacovelli */
/* Date: 6/24/2014          */
/****************************/

#include "ServerSock.h"
#include <stdio.h>
#include <stdlib.h>

#define TIME 10
#define CHARBUF 256

int main(int argc, char **argv) {
    int listenerfd, clientfd, socketLoop, strLen, maxfd;
    fd_set read_fds;
    struct timeval tv;
    char* str = new char[CHARBUF];

    if (argc < 2) {
        printf("Too few arguments!\n");
        printf("Try: serverLibTest <port>\n");
        return -1;
    }

    ServerSock sock(atoi(argv[1]));

    tv.tv_sec = TIME;
    tv.tv_usec = 0;

    listenerfd = sock.listenSock(5);

    printf("Server listening on port %s\n", argv[1]);

    while(1) {
        read_fds = *sock.getMasterfd();

        if(select(sock.getMaxfd()+1, &read_fds, NULL, NULL, &tv) == 0) {
            printf("timeout\n");
            sock.closeAllClients();
            tv.tv_sec = TIME;
        } else {
            maxfd = sock.getMaxfd();

            for(socketLoop=0; socketLoop<=maxfd; socketLoop++) {

                if (FD_ISSET(socketLoop, &read_fds)) {

                    if (socketLoop == listenerfd) {
                        clientfd = sock.acceptSock();
                        printf("Client @ %s on port %d has connected\n", sock.getClientIP(), sock.getClientPort());
                        sprintf(str, "You are connected!\n");

                        sock.sendSock(clientfd, str);
                    } else {
                        memset(&str[0], 0, CHARBUF);
                        strLen = sock.readSock(socketLoop, str, CHARBUF);

                        if (strLen <= 0) {
                            printf("Client disconnected!\n");
                            sock.closeClient(socketLoop);
                        } else {
                            str[strLen] = '\0';
                            printf("Rx: %x\n", (int)str);
                            sprintf(str ,"Recieved\n");
                            //sock.sendSock(socketLoop, str);
                            tv.tv_sec = TIME;
                        }
                    }
                }
            }
        }
    }

    return 0;
}
