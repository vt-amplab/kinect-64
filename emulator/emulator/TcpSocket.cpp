#include "stdafx.h"
#include "TcpSocket.h"
#include "Game.h"
#include "Player.h"

using namespace System;
using namespace System::Net::Sockets;
using namespace System::Text;
using namespace System::Threading;
using namespace System::Net;

// 192.168.1.4 playerBox
// 192.168.1.2 kinectBox
Server::Server()
{
	
}

Server::Server( SmashBros::Game* context)
{
	handler = gcnew ParameterizedThreadStart(this, &Server::HandleClient);
	m_context = context;
	tcpListener = gcnew TcpListener(IPAddress::Any, 9999);
	listenThread = gcnew Thread(gcnew ThreadStart(this, &Server::ListenForClients));
	listenThread->Start();
}
void Server::ListenForClients()
{
	tcpListener->Start();

	while (true)
	{
		//blocks until a client has connected to the server
		TcpClient^ client = tcpListener->AcceptTcpClient();

		//create a thread to handle communication 
		//with connected client
		Thread^ clientThread = gcnew Thread(handler);
		clientThread->Start(client);
	}
}

static int checkErr(int ec, const char* msg){
	if (ec != 0){
		Console::WriteLine(msg);
		return 1;
	}
	return 0;
}

void Server::HandleClient(System::Object^ tcpClient)
{
	NetworkStream^ clientStream = ((TcpClient^)tcpClient)->GetStream();

	//byte^ message = gcnew byte[4096];
	array< Byte >^ message = gcnew array< Byte >(4096);
	array< Byte >^ reply = gcnew array< Byte >(5);

	int bytesRead;
	Console::WriteLine("NEW CLIENT");
	while (true)
	{
		bytesRead = 0;

		try
		{
			//blocks until a client sends a message
			bytesRead = clientStream->Read(message, 0, 4096);
			Console::WriteLine("Read "+bytesRead);
			if (bytesRead <=0 ){
				//the client has disconnected from the server
				Console::WriteLine("CLIENT DISCONNECTED");
				break;
			}else if(bytesRead < 4){
				Console::WriteLine("INVALID BYTES RECIEVED! ");
				break;
			}
			int ec;
			switch(message[0]){
				case SmashBros::MCharacter:
					ec = m_context->pickCharacter((SmashBros::Character)message[1], (int)message[3]);

				break;
				case SmashBros::MAttack:
					ec = m_context->attack((SmashBros::Attack)message[2], (SmashBros::Direction)message[1], (int)message[3]);
					Console::Write("Attack: "+message[2]+"   Move: "+message[1]);
					if (checkErr(ec, "trying to fight in wrong state")){
					
					}
				break;
				case SmashBros::MMap:
					ec = m_context->pickMap((SmashBros::Map)message[2], (int)message[3]);
					if (checkErr(ec, "trying to change map in wrong state")){
					
					}
				break;
				default:
					Console::WriteLine("INVALID MODE RECIEVED! : "+message[0]);
				break;
			}
			reply[0] = 0;
			reply[1] = m_context->getState();
			clientStream->Write(reply, 0, 2);

		}
		catch(...)
		{
			//a socket error has occured
			Console::WriteLine("SOCKET ERROR");
			break;
		}


	}

	((TcpClient^)tcpClient)->Close();
}
