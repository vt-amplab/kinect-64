#include "ADC.h"

ADC::ADC(int channel1)
{
	channel = channel1;
}

int ADC::ReadValue() {
	FILE* fd;
	char* fileTemp = (char*)malloc(50);
	char tmp[4], temp[5], file[50];
	int value;

	strcpy(fileTemp, "/sys/devices/");
	fileTemp = fileHelper(fileTemp, "ocp");
	fileTemp = fileHelper(fileTemp, "helper");
	strcat(fileTemp, "AIN");	
	strcpy(file, fileTemp);
	sprintf(temp, "%d", channel);
	strcat(file, temp);

	fd = fopen(file, "r");
	value = atoi(fgets(tmp, 5, fd));

	fclose(fd);

	return value;
}

float ADC::ReadVoltage( float maxVoltage) {
	return ReadValue()*(maxVoltage/1800);
}

char* ADC::fileHelper(char* directory, const char* folder) {
	DIR *dir;
	struct dirent *dp;
	int found = 1;
	char temp[256];

	if ((dir = opendir(directory)) == NULL) {
		perror("Cannot open PATH");
	}

	while (((dp = readdir(dir)) != NULL) && found == 1) {
		if (strncmp(dp->d_name, folder, strlen(folder)) == 0) {
			strcpy(temp, directory);
			strcat(temp, dp->d_name);
			strcat(temp, "/");

			found = 0;
		}
	}
	closedir(dir);
	directory = temp;

	return directory;
}
