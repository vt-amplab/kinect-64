#include "SPI.h"

#include <stdlib.h>
#include <string.h>
#include <arpa/inet.h>
#include <fstream>
#include <math.h>
#include <limits>
#include <fcntl.h>
#include <sys/ioctl.h>
#include <linux/spi/spidev.h>
#include <unistd.h>
#include "SimpleGPIO.h"
#include <stdint.h>

//**********************************************************************************************
// SPI Write & Read Data
//**********************************************************************************************
void SPI::manual_chipSelect_on(int channel)
{
    if (num_cs>channel && num_cs > 0)
      gpio_set_value(manual_cs[channel], LOW);
}
void SPI::manual_chipSelect_init(int *cs_lines, int num_CS_lines)
{
    CS_manual = true;
    num_cs    = num_CS_lines;
    manual_cs = cs_lines;
    
    for (int i = 0; i<num_cs; i++) 
    {
        gpio_export(manual_cs[i]);
        gpio_set_dir(manual_cs[i], OUTPUT_PIN);
        gpio_set_value(manual_cs[i], HIGH);
    }
}
void SPI::manual_chipSelect_all_off()
{
    for (int i = 0; i<num_cs; i++) 
      gpio_set_value(manual_cs[i], HIGH);
}
//**********************************************************************************************
// SPI Write & Read Data
//**********************************************************************************************
bool SPI::spiWriteAndRead(char *in_data, int num_char_in, char *out_data, int num_char_out)
{   
    struct spi_ioc_transfer spi[2];
    int retVal = -1;
    bzero(&spi,sizeof(spi));

    spi[0].tx_buf = (unsigned long)in_data; //transmit from "data"
    spi[0].rx_buf = (unsigned long)in_data; //receive into "data"
    spi[0].len = sizeof(char)*num_char_in;
    spi[0].delay_usecs = 0;  
    spi[0].cs_change = 1;

    spi[1].tx_buf = (unsigned long)out_data; //transmit from "data"
    spi[1].rx_buf = (unsigned long)out_data; //receive into "data"
    spi[1].len = sizeof(char)*num_char_out;
    spi[1].delay_usecs = 0;  
    spi[1].cs_change = 1;

    retVal = ioctl(spi_fd, SPI_IOC_MESSAGE(2), &spi);

    if (retVal < 0){
        printf("Error - Problem transmitting spi data..ioctl");
        return false;
    }
    return true;
}
//**********************************************************************************************
// SPI Write & Read Data; manual chip select
//**********************************************************************************************
bool SPI::spiWriteAndRead(char *in_data, int num_char_in, char *out_data, int num_char_out, int CS_line)
{   
    struct spi_ioc_transfer spi;
    int retVal = -1;
    bzero(&spi,sizeof(spi));

    spi.tx_buf = (unsigned long)out_data; //transmit from "data"
    spi.rx_buf = (unsigned long)out_data; //receive into "data"
    spi.len = sizeof(char)*num_char_out;
    spi.delay_usecs = 0;  

    spiWrite(in_data, num_char_in, CS_line);

    manual_chipSelect_on (CS_line);
    
    retVal = ioctl(spi_fd, SPI_IOC_MESSAGE(1), &spi);
    
    manual_chipSelect_all_off();


    if (retVal < 0){
        printf("Error - Problem transmitting spi data..ioctl");
        return false;
    }
    return true;
}
//**********************************************************************************************
// SPI Write Data Only
//**********************************************************************************************
bool SPI::spiWrite(char *data, int num_char)
{   
    int retVal = -1;
    //for (int i =0; i<num_char;i++)
    //{
    //  printf("Sending : 0x%02x\n", data[i]);
    //}
    retVal = write(spi_fd, data, num_char);

    if (retVal < 0){
        printf("Error - Problem transmitting spi data..ioctl");
        return false;
    }
    return true;
}
//**********************************************************************************************
// SPI Write Data Only; manual chip select
//**********************************************************************************************
bool SPI::spiWrite(char *data, int num_char, int CS_line)
{   
    int retVal = -1;

    manual_chipSelect_on (CS_line);

    retVal = write(spi_fd, data, num_char);

    manual_chipSelect_all_off();

    if (retVal < 0){
        printf("Error - Problem transmitting spi data..ioctl");
        return false;
    }
    return true;
}
//**********************************************************************************************
// SPI Write & Read Data
//**********************************************************************************************
bool SPI::spiWriteChunks(char *data, int num_char_per_chunk, int num_chunks)
{   
    struct spi_ioc_transfer *spi = (struct spi_ioc_transfer*) malloc(sizeof(struct spi_ioc_transfer)*num_chunks);
    int retVal = -1;
    bzero(spi,sizeof(struct spi_ioc_transfer)*num_chunks);

    for (int i = 0; i<num_chunks; i ++)
    {
      spi[i].tx_buf = (unsigned long)data+(sizeof(char)*num_char_per_chunk*i); //transmit from "data"
      spi[i].rx_buf = (unsigned long)data+(sizeof(char)*num_char_per_chunk*i); //receive into "data"
      spi[i].len = sizeof(char)*num_char_per_chunk;
      spi[i].delay_usecs = 0;  
      spi[i].cs_change = 1;
    }

    retVal = ioctl(spi_fd, SPI_IOC_MESSAGE(num_chunks), spi);

    free(spi);

    if (retVal < 0){
        printf("Error - Problem transmitting spi data..ioctl\n");
        return false;
    }
    return true;
}
//**********************************************************************************************
// SPI OPEN PORT
//**********************************************************************************************
int SPI::OpenPort (int device) //spi_device 0=CS0, 1=CS1
{
    int status_value = -1;

    if (device)
    {
        spi_fd = open("/dev/spidev1.1", O_RDWR);
    }
    else
    {
        spi_fd = open("/dev/spidev1.0", O_RDWR);
    }

    if (spi_fd < 0)
    {
        printf("Error - Could not open SPI device\n");
    }

    status_value = ioctl(spi_fd, SPI_IOC_WR_MODE, &spi_mode);
    if (status_value < 0)
    {
        printf("Could not set SPIMode (WR)...ioctl fail\n");
    }

    status_value = ioctl(spi_fd, SPI_IOC_RD_MODE, &spi_mode);
    if (status_value < 0)
    {
        printf("Could not set SPIMode (RD)...ioctl fail\n");
    }

    status_value = ioctl(spi_fd, SPI_IOC_WR_BITS_PER_WORD, &spi_bitsPerWord);
    if (status_value < 0)
    {
        printf("Could not set SPI bitsPerWord (WR)..ioctl fail\n");
    }

    status_value = ioctl(spi_fd, SPI_IOC_RD_BITS_PER_WORD, &spi_bitsPerWord);
    if (status_value < 0)
    {
        printf("Could not set SPI bitsPerWord (RD)..ioctl fail\n");
    }

    status_value = ioctl(spi_fd, SPI_IOC_WR_MAX_SPEED_HZ, &spi_speed);
    if (status_value < 0)
    {
        printf("Could not set SPI speed (WR)..ioctl fail\n");
    }

    status_value = ioctl(spi_fd, SPI_IOC_RD_MAX_SPEED_HZ, &spi_speed);
    if (status_value < 0)
    {
        printf("Could not set SPI speed (RD)..ioctl fail\n");
    }

    return(status_value);
}
SPI::SPI(int spi_device) 
{
    CS_manual = false;
    num_cs = 0;
    
    // Set SPI Mode
    // SPI_MODE_0 (0,0) CPOL=0, CPHA=0
    // SPI_MODE_1 (0,1) CPOL=0, CPHA=1
    // SPI_MODE_2 (1,1) CPOL=1, CPHA=0
    // SPI_MODE_3 (1,1) CPOL=1, CPHA=1
    spi_mode = SPI_MODE_1;

    // Set Bits Per Word
    spi_bitsPerWord = 8;

    // Set SPI Bus Speed
    spi_speed = 16000000;
    
    OpenPort(spi_device);
}
SPI::SPI(int spi_device, unsigned char mode, unsigned char bitsPerWord, unsigned int speed) 
{
    CS_manual = false;
    num_cs = 0;
    
    // Set SPI Mode
    // SPI_MODE_0 (0,0) CPOL=0, CPHA=0
    // SPI_MODE_1 (0,1) CPOL=0, CPHA=1
    // SPI_MODE_2 (1,1) CPOL=1, CPHA=0
    // SPI_MODE_3 (1,1) CPOL=1, CPHA=1
    spi_mode = mode;

    // Set Bits Per Word
    spi_bitsPerWord = bitsPerWord;

    // Set SPI Bus Speed
    spi_speed = speed;
    
    OpenPort(spi_device);
}
//**********************************************************************************************
// SPI CLOSE PORT
//**********************************************************************************************
int SPI::spiClosePort()
{
    int status_value = -1;

    status_value = close(spi_fd);
    if(status_value < 0)
    {
        printf("Error - Could not close SPI device");
    }

    return(status_value);
}

SPI::~SPI() 
{
    spiClosePort();
}
