using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace PressPlay.FFWD
{
    public static class Time
    {
        public static float time = 0.0f;
        public static float deltaTime = 0.0f;
        public static float fixedDeltaTime = 0.0f;
        public static float timeScale = 1.0f;
        public static float realtimeSinceStartup = 0.0f;
        public static int frameCount = 0;
        
        internal static void Reset()
        {
            frameCount = 0;
            time = 0.0f;
            deltaTime = 0.0f;
            timeScale = 1.0f;
            realtimeSinceStartup = 0.0f;
        }

        internal static void Update(float elapsedSeconds)
        {
            deltaTime = elapsedSeconds * timeScale;
            frameCount++;
        }

        internal static void FixedUpdate(float elapsedSeconds, float totalSeconds)
        {            
            realtimeSinceStartup = totalSeconds;
            deltaTime = elapsedSeconds * timeScale;
            fixedDeltaTime = deltaTime;
            time += deltaTime;
        }
    }
}
