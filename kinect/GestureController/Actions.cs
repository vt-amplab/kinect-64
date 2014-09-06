using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using GestureRecognition;

using log4net;

namespace GestureController
{
    public class GestureToActionMapper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(GestureToActionMapper));
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
            {"Punch", new AttackMessage(Attack.Punch, Direction.NoMove)},
            {"Punch Up", new AttackMessage(Attack.Punch, Direction.MoveUp)},
            {"Punch Down", new AttackMessage(Attack.Punch, Direction.MoveDown)},
            {"Grab", new AttackMessage(Attack.Grab, Direction.NoMove)},
            {"Taunt", new AttackMessage(Attack.Taunt, Direction.NoMove)},
            {"Block", new AttackMessage(Attack.Block, Direction.NoMove)},
            {"Jump", new AttackMessage(Attack.NoAttack, Direction.MoveUp)},
            {"Crouch", new AttackMessage(Attack.NoAttack, Direction.MoveDown)},
            {"Move Left", new AttackMessage(Attack.NoAttack, Direction.MoveLeft)},
            {"Move Right", new AttackMessage(Attack.NoAttack, Direction.MoveRight)},
            {"Special", new AttackMessage(Attack.Special, Direction.NoMove)}
        };

        public void GestureRecognized(GestureRecognizedEventArgs args)
        {
            byte player = 0;
            if (args.AbsPosition.X > 0)
            {
                player = 1;
            }
            SendAction(args.Gesture.Label, player);
        }

        public void SendAction(string label, byte player)
        {
            Logger.Info("Recognized Gesture " + label + " by player " + (player + 1));
            if (comm.IsConnected)
            {
                Console.WriteLine("Sending " + label);
                AttackMessage mesg = Actions[label];
                mesg.SetPlayer(player);
                byte[] mesgBytes = mesg.GetMessageBytes();
                comm.Send(mesgBytes);
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
