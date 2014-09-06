using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureMapper
{
    public class GestureToActionMapper
    {
        public string[] Actions = new string[] { };
    }

    public class Message
    {
        public virtual byte[] GetMessageBytes();
    }

    public class Attack : Message
    {
    }

    public class CharacterChoice : Message
    {
    }

    public class MapChoice : Message
    {
    }
}
