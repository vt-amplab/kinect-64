#ifndef SPI_H
#define SPI_H

class SPI 
{
private:  //Variables
    //spi vars
    unsigned char spi_mode,spi_bitsPerWord;
    unsigned int spi_speed;
    int spi_fd;
    
    //Manual Chip select vars
    bool CS_manual;
    int *manual_cs;
    int num_cs;
    

private: //Functions
    int spiClosePort();
    int OpenPort (int device);    
    void manual_chipSelect_all_off();    
    void manual_chipSelect_on(int channel);    
    
public:
    void manual_chipSelect_init(int *cs_lines, int num_CS_lines);    
    
    bool spiWrite(char *data, int num_char);
    bool spiWriteChunks(char *data, int num_char_per_chunk, int num_chunks);
    bool spiWriteAndRead(char *in_data, int num_char_in, char *out_data, int num_char_out);

    bool spiWrite(char *data, int num_char, int CS_line);
    bool spiWriteAndRead(char *in_data, int num_char_in, char *out_data, int num_char_out, int CS_line);

    SPI(int spi_device);
    SPI(int spi_device, unsigned char mode, unsigned char bitsPerWord, unsigned int speed);
    ~SPI();
};

#endif
