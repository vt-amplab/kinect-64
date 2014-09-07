#include "I2C.h"
#include "GPIO.h"
#include "UART.h"
#include "ADC.h"
#include "LM73.h"
#include "ChipCap2.h"
#include <stdint.h>
#include <iostream>
#include <cstring>

#define led1 49
#define led2 68
#define led3 45
#define led4 44
#define led5 23
#define led6 67
#define led7 69
#define led8 66
#define ledNum 4


using namespace std;

#define BAUDRATE B19200


#define BUFSIZE 20
#define PACKETSIZE sizeof(MSG)


/*struct Test //UINT everythign in struct, attribute pack, decleare struct as pointer, char* as the same ampunt of memory as struct, cast struct as char*, 
{
	char* uart1;
	char* uart2;
	float adc0;
	float temp1;
	float temp2;
	float humid;
	U_INT8 gpiogood;
}
*/

int gpiotest ();
void getdata ();


typedef struct __attribute__((packed)) MSG
{



	uint8_t* uart1[BUFSIZE];
	uint8_t* uart2[BUFSIZE];
	int32_t adc0;
	int32_t temp1;
	int32_t temp2;
	int32_t humid;
	uint8_t gpiogood;
}MSG;

int size, tmp;

int main(int argc, char **argv, char **envp)
{
	MSG* newMsg = new MSG;

	LM73 temp1(1, 0x49);
	ChipCap2 cc2(2, 0x28);
	ADC adc(0);
	UART uart1(4);
	UART uart2(5);

	uart1.startbus(BAUDRATE, CS8 |CLOCAL | CREAD);
	uart1.writebus("Connected\n");

	uart2.startbus(BAUDRATE, CS8 |CLOCAL | CREAD);
	uart2.writebus("Connected\n");

	GPIO gpio
	int i;
	int ledin[] = {led1, led2, led3, led4};
	int ledout[] = {led5, led6, led7, led8};

	for (i = 0; i < ledNum; i++) {
		gpio_export(ledin[i]);
		gpio_set_dir(ledin[i], 1);
		gpio_set_value(ledin[i], 0);
		gpio_fd_open(ledin[i]);
	}

		for (i = 0; i < ledNum; i++) {
		gpio_export(ledout[i]);
		gpio_set_dir(ledout[i], 0);
		gpio_fd_open(ledout[i]);
	}



/////////////////////////////////////////////////////////////////////////////
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
//////////////////////////////////////////////////////////////////////////////////////





	while(1)
	{
		/*
		getdata ();

    	char* data2 = new char[PACKETSIZE];

		memset(data2, 0, PACKETSIZE);

		data2 = (char*)newMsg;//data2 = (char*)&newMsg;




        usleep (100000);

*/


      
        getdata ();

    	char* data2 = new char[PACKETSIZE];

		memset(data2, 0, PACKETSIZE);

		data2 = (char*)newMsg;//data2 = (char*)&newMsg;

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
                        memset(&str[0], 0, sizeof(str));
                        strLen = sock.readSock(socketLoop, str, CHARBUF);

                        if (strLen <= 0) {
                            printf("Client disconnected!\n");
                            sock.closeClient(socketLoop);
                        } else {
                            str[strLen] = '\0';
                            printf("Rx: %s\n", str);

                            if (str == "Feed Me")
                            {
                            sock.sendSock(socketLoop, data2);
                        	}
                        	else {}
                            tv.tv_sec = TIME;
                        }
                    }
                }
            }
        }






	}



return 0;
}



int gpiotest ()
{
	for (i = 0; i < ledNum; i++) {
		gpio_set_value(ledin[i], 1);
	}

	int test = (gpio_get_value(led4)&gpio_get_value(led5)&gpio_get_value(led6)&gpio_get_value(led7)&gpio_get_value(led8));

		for (i = 0; i < ledNum; i++) {
		gpio_set_value(ledin[i], 0);
	}

	return test;

}

void getdata ()
{
		newMsg->temp1=(int32_t)temp1.gettemperature(); //gets LM sensor temp
		newMsg->humid=(int32_t)cc2.getHumidity(); //gets cc2 temp
		newMsg->temp2=(int32_t)cc2.getTemperature();//gets cc2 humdity
		newMsg->adc0=adc.ReadValue();//adc value
		newMsg->gpiogood = gpiotest ();


				printf("bytes: %d\n", tmp);
                size = uart1.readbus(buffer, 255);
                buffer[size-1] = '\0';

                printf("Rx: %s\n", buffer);
                strcpy(newMsg->uart1, buffer);


                tmp = uart1.writebus(buffer);

                printf("bytes: %d\n", tmp);
                size = uart2.readbus(buffer, 255);
                buffer[size-1] = '\0';
                strcpy(newMsg->uart2, buffer);

                //strcpy(newMsg->message, "hello from server\0");

                printf("Rx: %s\n", buffer);

                tmp = uart2.writebus(buffer);



}
