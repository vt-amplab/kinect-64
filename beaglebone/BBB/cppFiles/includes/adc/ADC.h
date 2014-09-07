#ifndef ADC_H_
#define ADC_H_

#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <dirent.h>
#include <string.h>

class ADC
{
private:
int channel;

public:
ADC(int channel1);


int ReadValue();
float ReadVoltage( float maxVoltage);
char* fileHelper(char* directory, const char* folder);
};

#endif
