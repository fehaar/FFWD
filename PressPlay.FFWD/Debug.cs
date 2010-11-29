using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public class Debug
    {
        public static void Log(object message)
        {
            System.Diagnostics.Debug.WriteLine(message.ToString());
        }
    }
}
