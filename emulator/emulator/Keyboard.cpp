#include "stdafx.h"
#include "Keyboard.h"
#include "windows.h"

using namespace System::Diagnostics;

Keyboard::Keyboard() : m_delay(20){

}

Keyboard::~Keyboard(){

}
void Keyboard::tapKey(char key, int delay){
	char buf[2] = {key, '\0'};
	return tapKey(buf, delay);
}

void Keyboard::tapKey(const char* key, int delay){

	for (const char* i = key; *i; i++)
		keybd_event(getKeyCode(*i), 0,0,NULL);
	Sleep(delay ? delay : m_delay );
	for (const char* i = key; *i; i++)
		keybd_event(getKeyCode(*i), 0,KEYEVENTF_KEYUP,NULL);
	Sleep(delay ? delay : m_delay );
}

void Keyboard::up(int dur){
	
}
void Keyboard::down(int dur){

}
void Keyboard::left(int dur){

}
void Keyboard::right(int dur){

}


int Keyboard::getKeyCode(char key){
	switch(key){
	case 'x': return 0x58;
	case 'c': return 0x43;
	case 'y': return 0x59;
	case 'u': return 0x55;
	case VK_RETURN:
	case VK_LEFT:
	case VK_RIGHT:
	case VK_UP:
		return key;
	}
	return key-32;
}