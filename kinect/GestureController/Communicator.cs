using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;

using log4net;

namespace GestureController
{

    public class CommunicatorStateObject
    {
        public Socket workSocket = null;
        public const int MaxBufferSize = 256;
        public byte[] buffer = new byte[MaxBufferSize];
        public int BufferSize = 0;
    }

    public class Communicator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Communicator));
        public delegate void DataReceived(byte[] buffer, int bufferLength);
        public event DataReceived DataReceivedEvent;

        private bool _connected;
        private bool _connecting;

        private IPEndPoint _remoteEP;

        public bool IsConnected
        {
            get
            {
                return _connected;
            }
        }
        public bool IsConnecting
        {
            get
            {
                return _connecting;
            }
        }

        private Socket _client;

        public void StartClient(IPAddress address, int port)
        {
            _remoteEP = new IPEndPoint(address, port);
            ConnectToClient();
        }

        private void ConnectToClient() 
        {
            try
            {
                _connecting = true;
                Logger.Debug("Beginning Async Kinect");

                bool success = false;
                while (!success)
                {
                    _client = new Socket(_remoteEP.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

                    IAsyncResult result = _client.BeginConnect(_remoteEP,
                        new AsyncCallback(ConnectCallback), _client);
                    result.AsyncWaitHandle.WaitOne(5000);
                    success = _client.Connected;
                    if (!success)
                    {
                        _client.Close();
                        Logger.Debug("Connection Failed -- will try again");
                    }
                }

                Logger.Debug("Connected to server");

                Receive();
            }
            catch (Exception e)
            {
                Logger.Debug("Exception encountered", e);
            }
        }

        public void StopClient()
        {
            try
            {
                _client.Disconnect(true);
                _connected = false;
            }
            catch (Exception e)
            {
                Logger.Debug("Exception encountered", e);
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                _client.BeginSend(data, 0, data.Length, 0,
                    new AsyncCallback(SendCallback), _client);
            }
            catch (SocketException se)
            {
                Logger.Debug("Caught serial exception", se);
            }
        }

        public void Receive()
        {
            try
            {
                CommunicatorStateObject state = new CommunicatorStateObject();
                state.workSocket = _client;

                _client.BeginReceive(state.buffer, 0,
                    CommunicatorStateObject.MaxBufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (SocketException e)
            {
                Logger.Debug("Exception encountered in Receive", e);
                ConnectToClient();
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);
                Logger.Debug("Connect completed");
                _connected = true;
                _connecting = false;
            }
            catch (ObjectDisposedException e)
            {
                Logger.Debug("Caught a disposed exception in Connect Callback");
            }
            catch (SocketException e)
            {
                Logger.Debug("Caught a socket exception in Connect Callback", e);
                ConnectToClient();
            }
            catch (Exception e)
            {
                Logger.Debug("Caught an exception", e);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                CommunicatorStateObject state = (CommunicatorStateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                Logger.Debug("Received " + bytesRead + " bytes from server.");

                if (bytesRead > 1 && DataReceivedEvent != null)
                {
                    Logger.Debug("Finished receving data from server");
                    DataReceivedEvent(state.buffer, bytesRead);
                }

                Receive();
            }
            catch (SocketException e)
            {
                Logger.Debug("Exception encountered", e);
                ConnectToClient();
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server", bytesSent);
            }
            catch (SocketException e)
            {
                Logger.Debug("Exception encountered", e);
                ConnectToClient();
            }
        }
    }

}
