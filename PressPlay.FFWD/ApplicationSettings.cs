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
        public static bool ShowiTweenUpdateTime = false;
        public static bool ShowTurnOffTime = true;
        public static bool ShowTimeBetweenUpdates = true;
        public static bool ShowRaycastTime = true;
        public static bool ShowParticleAnimTime = true;
        public static bool ShowPerformanceBreakdown = true;
        public static bool ShowFPSCounter = true;
        public static bool ShowDebugDisplays = true;
        public static bool ShowDebugLines = false;
        public static string DebugLineCamera = "";
        public static SpriteFont DebugFont;
    }
}
