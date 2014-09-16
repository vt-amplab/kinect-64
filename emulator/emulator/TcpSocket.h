#pragma once

#include "Game.h"

using namespace System;
using namespace System::Net::Sockets;
using namespace System::Threading;

namespace SmashBros{
class Game;
}



public ref class Server
{
public:
	TcpListener^ tcpListener;
	Thread^ listenThread;
	System::Threading::ParameterizedThreadStart^ handler;
	
	SmashBros::Game* m_context;

	TcpClient^ playerBox;
	TcpClient^ kinectBox;


	Server();
	Server(SmashBros::Game* context);
	
	void ListenForClients();

	void HandleClient(System::Object^ tcpClient);
};