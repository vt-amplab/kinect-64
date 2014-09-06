#include "stdafx.h"
#pragma once

class Keyboard{
public:
	Keyboard();
	~Keyboard();

	virtual void tapKey(char key, int delay=0);
	virtual void tapKey(const char* key, int delay=0);

	virtual void up(int dur=50);
	virtual void down(int dur=50);
	virtual void left(int dur=50);
	virtual void right(int dur=50);
	
	static int getKeyCode(char key);

private:
	int m_delay;
	long long m_ts;

};