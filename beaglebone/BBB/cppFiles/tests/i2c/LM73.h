#ifndef LM73_H_
#define LM73_H_

#include "I2C.h"

//int* lm73Init(int addr, int bus);
//float lm73GetTemperature(int* i2cfd);
//void lm73Close(int* i2cfd);

class LM73 {
private:
        I2C i2c;
public:
        LM73(const int addr);
        LM73(const int bus, const int addr);
        ~LM73();
        float getTemperature();
        void close();
};


#endif