using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class Debug
    {
        public static void Log(string message)
        {
#if WINDOWS
            Console.WriteLine(message);
#endif
        }
    }
}
