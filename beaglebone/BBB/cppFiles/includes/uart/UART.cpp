#include "UART.h"

UART::UART(const int bus) {
	char* device = new char[50];
    sprintf(device, "/dev/ttyO%d", bus);
/*
	if ((uartfd = open(device, O_RDWR)) < 0) {
		sprintf(device, "Failed to open %s", device);
		perror(device);
		delete[] device;
	}
	*/
	uartfd = open(device, O_RDWR | O_NOCTTY );
    if (uartfd < 0) 
    {
    	perror(device); exit(-1); 
    	delete[] device;
    }
    delete[] device;
}

int UART::startbus(int BAUDRATE, int FLAGS) {
	/*
	char* addr = new char[50];

	sprintf(addr, "0x%x", address);

	if (ioctl(uartfd, I2C_SLAVE, address) < 0) {
		sprintf(addr, "Start error with address %s", addr);
        perror(addr);
        delete[] addr;
        return -1;
    }
    delete[] addr;
    */

    struct termios newtio;
    

     bzero(&newtio, sizeof(newtio)); /* clear struct for new port settings */

    /* BAUDRATE: Set bps rate. You could also use cfsetispeed and cfsetospeed.
       CRTSCTS : output hardware flow control (only used if the cable has
                 all necessary lines. See sect. 7 of Serial-HOWTO)
       CS8     : 8n1 (8bit,no parity,1 stopbit)
       CLOCAL  : local connection, no modem contol
       CREAD   : enable receiving characters */
       
    newtio.c_cflag = BAUDRATE | FLAGS;

    /* IGNPAR  : ignore bytes with parity errors
       otherwise make device raw (no other input processing) */
       
    newtio.c_iflag = IGNPAR;

    /*  Raw output  */
    
    newtio.c_oflag = 0;

    /* ICANON  : enable canonical input
       disable all echo functionality, and don't send signals to calling program */
       
    newtio.c_lflag = ICANON;
    
    /* now clean the modem line and activate the settings for the port */
    
    tcflush(uartfd, TCIFLUSH);
    
    tcsetattr(uartfd,TCSANOW,&newtio);
    

    return 0;
}

int UART::readbus(char* data, const int bytes) {
	int tmp;

	tmp = read(uartfd, data, bytes);

	if (tmp != bytes)
        perror("Read did not return bytes specified");

    return tmp;
}

int UART::writebus(char* data) {
	return write(uartfd, data, sizeof(data));
}

void UART::closebus() {
	close(uartfd);
}

UART::~UART() {
	closebus();
}
