using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PressPlay.FFWD
{
#if DEBUG
    public static class DebugSettings
    {
        public static bool LogActivatedComponents = false;
        public static bool LogAssetLoads = false;
        public static bool LogSendMessage = false;
    }
#endif
}
