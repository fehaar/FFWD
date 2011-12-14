using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD
{
    public static class ApplicationSettings
    {
        public enum To2dMode { DropX, DropY, DropZ };
        public static To2dMode to2dMode = To2dMode.DropY;
        public static bool ShowComponentProfile = false;
        public static bool ShowTurnOffTime = false;
        public static bool ShowTimeBetweenUpdates = false;
        public static bool ShowRaycastTime = false;
        public static bool ShowParticleAnimTime = false;
        public static bool ShowPerformanceBreakdown = true;
        public static bool ShowFPSCounter = true;
        public static bool ShowBodyCounter = true;
        public static bool ShowDebugDisplays = true;
        public static bool ShowDebugLines = false;
        public static bool ShowDebugPhysics = false;
        public static bool LogActivatedComponents = false;
        public static SpriteFont DebugFont;
#if WINDOWS
        public static int AssetLoadInterval = 50; // In Milliseconds
#else
        public static int AssetLoadInterval = 50; // In Milliseconds
#endif

        public static class DefaultCapacities
        {
            /// <summary>
            /// All of these settings can be tuned in order to adjust memory usage of the engine.
            /// You should do this in your game contructor.
            /// </summary>
            #region Default list sizes
            public static int Lights = 4;
            public static int QueryHelper = 20;
            public static int RaycastHits = 20;
            public static int GestureSamples = 4;
            public static int Touches = 4;
            #endregion
        }

    }
}
