#ifndef CHIPCAP2_H_
#define CHIPCAP2_H_

#include "i2c.h"

int* cc2Init(int addr, int bus);
float cc2GetHumidity(int* i2cfd);
float cc2GetTemperature(int* i2cfd);
void cc2Close(int* i2cfd);

#endif