using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using GestureRecognition;

namespace GestureFormatConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                BinaryFormatter serializer = new BinaryFormatter();
                try
                {
                    Stream file = File.OpenRead(arg);
                    Dictionary<string, Gesture> gestures = (Dictionary<string, Gesture>)serializer.Deserialize(file);
                    file.Close();

                    Dictionary<Guid, Gesture> _gestures = new Dictionary<Guid, Gesture>();

                    foreach (Gesture gesture in gestures.Values)
                    {
                        _gestures[gesture.ID] = gesture;
                    }

                    file = File.OpenWrite(arg);
                    serializer.Serialize(file, _gestures);
                    file.Flush();
                    file.Close();
                }
                catch (SerializationException exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
        }
    }
}
