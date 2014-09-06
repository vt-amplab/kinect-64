using System;
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

        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        private bool _connected;
        private const int DefaultConnectTimeout = 5000;

        public bool IsConnected
        {
            get
            {
                return _connected;
            }
        }

        private Socket _client;

        public void StartClient(IPAddress address, int port)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(address, port);

                _client = new Socket(address.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                Logger.Debug("Beginning Async Kinect");
                _client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), _client);
                connectDone.WaitOne(DefaultConnectTimeout);

                Receive();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
                Console.WriteLine(e.ToString());
            }
        }

        public void Send(byte[] data)
        {
            sendDone.Reset();
            _client.BeginSend(data, 0, data.Length, 0,
                new AsyncCallback(SendCallback), _client);
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
                receiveDone.Reset();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void WaitForReceive()
        {
            receiveDone.WaitOne();
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                Logger.Debug("Connect completed");
                connectDone.Set();
                _connected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                CommunicatorStateObject state = (CommunicatorStateObject)ar.AsyncState;
                Socket client = state.workSocket;

                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    Console.WriteLine("Received {0} bytes in response", bytesRead);
                    state.BufferSize += bytesRead;
                    client.BeginReceive(state.buffer, state.BufferSize,
                        CommunicatorStateObject.MaxBufferSize - state.BufferSize,
                        0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    if (state.BufferSize > 1 && DataReceivedEvent != null)
                    {
                        DataReceivedEvent(state.buffer, state.BufferSize);
                    }
                    receiveDone.Set();
                    Receive();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server", bytesSent);

                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

}
