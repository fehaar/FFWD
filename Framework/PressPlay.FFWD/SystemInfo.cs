using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PressPlay.FFWD
{
    public static class SystemInfo
    {
        public static int processorCount
        {
            get
            {
                return Environment.ProcessorCount;
            }
        }
    }
}
