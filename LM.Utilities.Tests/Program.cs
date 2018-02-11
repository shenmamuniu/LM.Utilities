using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LM.Utilities
{
    class Program
    {
        static void Main(string[] args)
        {
            //Utilities.WinTools.CheckPythonExist();
            //List<string> appList= WinTools.GetAllApps();
            //for (int i = 0; i < appList.Count; i++)
            //{
            //    Debug.WriteLine(appList[i]);
            //    Console.WriteLine(appList[i]);
            //}


            IntPtr ptr2=WinTools.GetMemory("lm.utilities.samples");
            IntPtr ptr = Process.GetCurrentProcess().MainWindowHandle;
            if (ptr2!=IntPtr.Zero)
            {
                Console.WriteLine("the app has started.");
            }
            else {
                Console.WriteLine("use memory map file");
                WinTools.WriteMemory("lm.utilities.samples", ptr);
            }
            Console.ReadKey();
        }
    }
}
