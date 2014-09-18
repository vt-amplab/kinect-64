#include "stdafx.h"
#include "Game.h"
#include "windows.h"
#include "Tracker.h"

using namespace System;
using namespace System::Net::Sockets;
using namespace System::Text;
using namespace System::Threading;
using namespace System::Net;
using namespace SmashBros;


int main(){
	// cv::Mat a;
	Tracker a;
	Sleep(2000);
	SmashBros::Game Game;

	// Game.test();
	
	while(1){
		int win = a.checkGameOver();
		switch(win){
		case 0: 
			Console::WriteLine("P1 wins!");
			Game.win(win);
		break;
		case 1:
			Console::WriteLine("P2 wins!");
			Game.win(win);
		break;
		}
		Sleep(500);
	}
	


	return 0;

}