#include "GPIO.h"
#include <sys/ioctl.h>


int main(int argc, char **argv, char **envp)
{
	GPIO gpio1(49);
	GPIO gpio2(68);

	gpio1.gpio_export();
	gpio2.gpio_export();

	gpio1.gpio_set_dir(1);
	gpio2.gpio_set_dir(1);	

	gpio1.gpio_set_value(0);
	gpio2.gpio_set_value(0);	

	while (1)
	{
		gpio1.gpio_set_value(1);
		gpio2.gpio_set_value(1);	

		

		usleep (1000000);

	gpio1.gpio_set_value(0);
	gpio2.gpio_set_value(0);

	usleep (1000000);



	}



}