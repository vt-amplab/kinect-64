using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Kinect;
using GestureRecognition;

namespace GestureMapper
{
    class Program
    {
        private const string GestureSaveFileLocation = @".\";

        static void Main(string[] args)
        {
            Communicator comms = new Communicator();
            comms.StartClient(new byte[] { 192, 168, 1, 6 }, 9999);
            GestureToActionMapper mapper = new GestureToActionMapper(comms);

            KinectSensor sensor = null;
            GestureRecognizer recognizer;

            foreach (var potential in KinectSensor.KinectSensors)
            {
                if (potential.Status == KinectStatus.Connected)
                {
                    sensor = potential;
                }
            }

            if (sensor != null)
            {

                recognizer = new GestureRecognizer(.9, 3);
                recognizer.GestureRecognized += mapper.GestureRecognized;


                // Open document
                Stream file = File.OpenRead(GestureSaveFileLocation + "fonte.bin");
                recognizer.RebuildGesturesFromBinary(file);
                file.Close();
                Console.WriteLine("Gestures Loaded!!");

                sensor.SkeletonStream.Enable();
                sensor.SkeletonFrameReady += recognizer.FrameReadyHandler;

                try
                {
                    sensor.Start();
                    //while (true)
                    //{
                    //    foreach (string gesture in GestureToActionMapper.Actions.Keys)
                    //    {
                    //        mapper.SendAction(gesture, 0);
                    //        Thread.Sleep(1000);
                    //    }
                    //    Thread.Sleep(2000);
                    //}
                    Thread.Sleep(60000);
                    comms.WaitForReceive();
                    comms.StopClient();
                    sensor.Stop();
                }
                catch (IOException exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
            else
            {
                Console.WriteLine("No sensor");
            }
        }
    }
}
