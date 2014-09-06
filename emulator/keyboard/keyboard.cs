using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;
using System.Runtime.InteropServices;
using System.Drawing;
namespace keyboard
{
    class keyboard
    {
        private static System.Timers.Timer aTimer;
        public static void Main()
        {
            System.Console.WriteLine("Hello, World!");
            aTimer = new System.Timers.Timer(2000);

            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;

            Console.WriteLine("Press the Enter key to exit the program... ");
            Console.ReadLine();
            Console.WriteLine("Terminating the application...");

        }

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Inputting an X...");
            try
            {
                SetForegroundWindow((IntPtr)0xfff);
                SendKeys.Send("{ENTER}");
            }
            catch (InvalidOperationException err)
            {
                Console.WriteLine("InvalidOperationException");
            }
            catch(ArgumentException err2)
            {
                Console.WriteLine("ArgumentException");
            }
            
        }
    }
}
