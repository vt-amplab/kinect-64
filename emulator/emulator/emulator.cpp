#include "stdafx.h"
#include "player.h"
#include "windows.h"

using namespace System;
using namespace System::Net::Sockets;
using namespace System::Text;
using namespace System::Threading;
using namespace System::Net;
using namespace SmashBros;

public ref class Server
{
public:
	TcpListener^ tcpListener;
	Thread^ listenThread;

	Server()
	{
		tcpListener = gcnew TcpListener(IPAddress::Any, 3000);
		listenThread = gcnew Thread(gcnew ThreadStart(this, ListenForClients));
		listenThread->Start();
	}
	void ListenForClients()
	{
		tcpListener->Start();

		while (true)
		{
			//blocks until a client has connected to the server
			TcpClient^ client = tcpListener->AcceptTcpClient();

			//create a thread to handle communication 
			//with connected client
			Thread^ clientThread = gcnew Thread(gcnew ParameterizedThreadStart(this, HandleClientComm));
			clientThread->Start(client);
		}
	}

	void HandleClientComm(Object client)
	{
		TcpClient tcpClient = (TcpClient)client;
		NetworkStream clientStream = tcpClient.GetStream();

		byte[] message = new byte[4096];
		int bytesRead;

		while (true)
		{
			bytesRead = 0;

			try
			{
				//blocks until a client sends a message
				bytesRead = clientStream.Read(message, 0, 4096);
			}
			catch
			{
				//a socket error has occured
				break;
			}

			if (bytesRead == 0)
			{
				//the client has disconnected from the server
				break;
			}

			//message has successfully been received
			ASCIIEncoding encoder = new ASCIIEncoding();
			System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));
		}

		tcpClient.Close();
	}
}

int main(){
	System::Net::IPAddress^ ipAddress = (System::Net::IPAddress^)System::Net::Dns::Resolve("0.0.0.0" )->AddressList[ 0 ];
	System::Net::IPEndPoint^ ipLocalEndPoint = gcnew System::Net::IPEndPoint( ipAddress,9999 );
	Console::WriteLine(ipAddress);
	TcpClient^ tcpClientA = gcnew TcpClient( ipLocalEndPoint );
	// tcpClientA;
	Sleep(60*1000);
	return 0;
	Sleep(3000);
	Player p1('w','s','a','d','x','c','z','v','b', VK_RETURN);
	Player p2('t','g','f','h','u','i','y','o','p', VK_BACK);

	p1.enterVersus();

	p1.pickCharacter(Mario); 
	p2.pickCharacter(DonkeyKong); 
	p1.start();
	p1.pickMap(SmashBros::HyruleCastle);
	Sleep(6000);
	while(1)
		for(int i = 0; i<Block+1; i++){
			for(int j = 0; j<MoveRight+1; j++){
				Sleep(1200);
				Console::WriteLine("Attack: "+i+", "+j);
				p1.attack((Attack)i, (Direction)j);
			}
		}

		return 0;

}