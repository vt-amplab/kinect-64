using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using GestureRecognition;
using System.Net;
using System.Net.Sockets;

namespace GestureMapper
{
    public class GestureToActionMapper
    {
        private Communicator comm;
        public GestureToActionMapper(Communicator comm)
        {
            this.comm = comm;
        }
        public static readonly Dictionary<string, AttackMessage> Actions = new Dictionary<string, AttackMessage>
        {
            {"Smash Right", new AttackMessage(Attack.Special, Direction.MoveRight)},
            {"Smash Left", new AttackMessage(Attack.Special, Direction.MoveLeft)},
            {"Smash Up", new AttackMessage(Attack.Special, Direction.MoveUp)},
            {"Smash Down", new AttackMessage(Attack.Special, Direction.MoveDown)},
            {"Punch Right", new AttackMessage(Attack.Punch, Direction.MoveRight)},
            {"Punch Left", new AttackMessage(Attack.Punch, Direction.MoveLeft)},
            {"Punch Up", new AttackMessage(Attack.Punch, Direction.MoveUp)},
            {"Punch Down", new AttackMessage(Attack.Punch, Direction.MoveDown)},
            {"Grab", new AttackMessage(Attack.Grab, Direction.NoMove)},
            {"Taunt", new AttackMessage(Attack.Taunt, Direction.NoMove)},
            {"Block", new AttackMessage(Attack.Block, Direction.NoMove)},
            {"Jump", new AttackMessage(Attack.NoAttack, Direction.MoveUp)},
            {"Crouch", new AttackMessage(Attack.NoAttack, Direction.MoveDown)},
            {"Move Left", new AttackMessage(Attack.NoAttack, Direction.MoveLeft)},
            {"Move Right", new AttackMessage(Attack.NoAttack, Direction.MoveRight)}
        };

        public void GestureRecognized(GestureRecognizedEventArgs args)
        {
            SendAction(args.Gesture.Label, 0);
        }

        public void SendAction(string label, byte player)
        {
            Console.WriteLine("Sending " + label);
            AttackMessage mesg = Actions[label];
            mesg.SetPlayer(player);
            byte[] mesgBytes = mesg.GetMessageBytes();
            comm.Send(mesgBytes);
        }

    }

    public class CommunicatorStateObject
    {
        public Socket workSocket = null;
        public const int MaxBufferSize = 256;
        public byte[] buffer = new byte[MaxBufferSize];
        public int BufferSize = 0;
    }

    public class Communicator
    {
        public delegate void DataReceived(byte[] buffer, int bufferLength);
        public event DataReceived DataReceivedEvent;

        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);

        private Socket _client;

        public void StartClient(byte[] address, int port)
        {
            try
            {

                IPAddress ipAddress = new IPAddress(address);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                _client = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                _client.BeginConnect(remoteEP,
                    new AsyncCallback(ConnectCallback), _client);
                connectDone.WaitOne();

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
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
                connectDone.Set();
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

    public struct PackedMessage
    {
        public byte Type;
        public byte movement;
        public byte attack;
        public byte player_selection;
    }

    public abstract class Message
    {
        protected Mode mode;
        protected byte player;

        public void SetPlayer(byte player)
        {
            this.player = player;
        }

        public abstract byte[] GetMessageBytes();
    }

    public class AttackMessage : Message
    {
        private Attack attack;
        private Direction direction;

        public AttackMessage(Attack attack, Direction dir)
        {
            this.mode = Mode.MAttack;
            this.attack = attack;
            this.direction = dir;
        }

        public override byte[] GetMessageBytes()
        {
            return new byte[] { (byte)mode, (byte)direction, (byte)attack, player };
        }
    }

    public class CharacterChoiceMessage : Message
    {
        private Character character;

        public CharacterChoiceMessage(Character character)
        {
            this.mode = Mode.MCharacter;
            this.character = character;
        }

        public override byte[] GetMessageBytes()
        {
            return new byte[] { (byte)mode, (byte)character, 0, player };
        }
    }

    public class MapChoiceMessage : Message
    {
        private Map map;

        public MapChoiceMessage(Map map)
        {
            this.mode = Mode.MMap;
            this.map = map;
        }

        public override byte[] GetMessageBytes()
        {
            return new byte[] { (byte)mode, (byte)map, 0, player };
        }
    }

    public enum Mode {
        MCharacter = 0,
        MAttack,
        MMap
    }

    public enum Character
    {
        Luigi = 0,
        Mario,
        DonkeyKong,
        Link,
        Samus,
        CFalcon,
        Ness,
        Yoshi,
        Kirby,
        Fox,
        Pikachu,
        Jigglypuff
    }

    public enum Attack
    {
        NoAttack = 0,
        Punch,
        Special,
        Taunt,
        Grab,
        Block
    }

    public enum Direction
    {
        NoMove = 0,
        MoveUp,
        MoveDown,
        MoveLeft,
        MoveRight
    }

    public enum Map
    {
        PeachCastle = 0,
        Jungle,
        HyruleCastle,
        Zebes,
        Mushroom,
        YoshiIsland,
        DreamLand,
        SectorZ,
        SaffronCity,
        Random
    }
}
