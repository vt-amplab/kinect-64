#ifndef ADC_H_
#define ADC_H_

#include <stdio.h>
#include <stdlib.h>
#include <sys/types.h>
#include <dirent.h>
#include <string.h>

int ADCReadValue(int channel);
float ADCReadVoltage(int channel, float maxVoltage);
char* fileHelper(char* directory, char* folder);

#endif