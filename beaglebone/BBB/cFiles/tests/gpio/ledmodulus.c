#include "gpio.h"
#include <sys/ioctl.h>

#define led1 49
#define led2 68
#define led3 45
#define led4 44
#define led5 23
#define led6 67
#define led7 69
#define led8 66
#define ledNum 8

int main(int argc, char **argv, char **envp)
{
	int i;
	int led[] = {led1, led2, led3, led4, led5, led6, led7, led8};

	for (i = 0; i < ledNum; i++) {
		gpio_export(led[i]);
		gpio_set_dir(led[i], 1);
		gpio_set_value(led[i], 0);
		gpio_fd_open(led[i]);
	}
	
	while (1)
	{
		for(i = 0; i < 256; i++) {
			//printf("i: %d\n1: %d, 2: %d, 3: %d, 4: %d, 5: %d, 6: %d, 7: %d, 8: %d\n", i, i&0x01, (i&0x02) >> 1, (i&0x03) >> 2, (i&0x04) >> 3, (i&0x05) >> 4, (i&0x06) >> 5, (i&0x07) >> 6, (i&0x08) >> 7);
			gpio_set_value(led[0], i&0x01);
			gpio_set_value(led[1], (i&0x02) >> 1);
			gpio_set_value(led[2], (i&0x04) >> 2);
			gpio_set_value(led[3], (i&0x08) >> 3);
			gpio_set_value(led[4], (i&0x10) >> 4);
			gpio_set_value(led[5], (i&0x20) >> 5);
			gpio_set_value(led[6], (i&0x40) >> 6);
			gpio_set_value(led[7], (i&0x80) >> 7);
			usleep (300000);
		}
	}

	return 0;
}