using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PressPlay.FFWD
{
    public class Profiler
    {
        [Conditional("DEBUG")]
        public static void BeginSample(string name)
        {
        }

        [Conditional("DEBUG")]
        public static void EndSample()
        {
        }
    }
}
