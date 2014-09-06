#include "stdafx.h"
#include "Game.h"
#include "windows.h"

using namespace System;
using namespace System::Net::Sockets;
using namespace System::Text;
using namespace System::Threading;
using namespace System::Net;
using namespace SmashBros;


int main(){
	Sleep(2000);
	SmashBros::Game Game(Fighting);

	Sleep(2000 * 60);
	
#if 0
	static int pp = 0;
	while(1)
		for(int i = 0; i<SmashBros::Block+1; i++){
			for(int j = 0; j<MoveRight+1; j++){
				// Sleep(1200);
				// Console::WriteLine("Attack: "+i+", "+j);
				Game.attack((Attack)i, (Direction)j, pp++ % 2);
			}  
		}
#endif

		return 0;

}