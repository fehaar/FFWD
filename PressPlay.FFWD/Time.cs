using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public static class Time
    {
        public static float time = 0.0f;
        public static float deltaTime = 0.0f;
        public static float actualDeltaTime = 0.0f;
        public static float timeScale = 1.0f;
        public static float realtimeSinceStartup = 0.0f;
        public static float timeSinceLastDraw = 0.0f;
        private static float lastDrawCall = 0.0f;

        internal static void Reset()
        {
            time = 0.0f;
            deltaTime = 0.0f;
            actualDeltaTime = 0.0f;
            timeScale = 1.0f;
            realtimeSinceStartup = 0.0f;
            timeSinceLastDraw = 0.0f;
            lastDrawCall = 0.0f;
        }

        internal static void Draw()
        {
            timeSinceLastDraw = time - lastDrawCall;
            lastDrawCall = time;
        }

        internal static void Update(float elapsedSeconds, float totalSeconds)
        {
            deltaTime = elapsedSeconds * timeScale;
            actualDeltaTime = elapsedSeconds;
            realtimeSinceStartup = totalSeconds;
            time += deltaTime;
        }
    }
}
