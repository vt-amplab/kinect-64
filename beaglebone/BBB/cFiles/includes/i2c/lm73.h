#ifndef LM73_H_
#define LM73_H_

#include "i2c.h"

int* lm73Init(int addr, int bus);
float lm73GetTemperature(int* i2cfd);
void lm73Close(int* i2cfd);

#endif