using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace PressPlay.FFWD
{
    public static class ApplicationSettings
    {
        public enum To2dMode { DropX, DropY, DropZ };
        public static To2dMode to2dMode = To2dMode.DropY;

        public static bool ShowPerformanceBreakdown = true;
        public static bool ShowFPSCounter = true;
        public static bool ShowDebugDisplays = true;
        public static SpriteFont DebugFont;
    }
}
