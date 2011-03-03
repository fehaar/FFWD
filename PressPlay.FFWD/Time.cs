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
        public static float actualDeltaTime = 0.0f;
        public static float timeScale = 1.0f;
        public static float realtimeSinceStartup = 0.0f;
        public static float timeSinceLastDraw = 0.0f;
        private static float lastDrawCall = 0.0f;

        private static float _fixedDeltaTime = (float)TimeSpan.FromSeconds(1.0 / 30.0).TotalSeconds;
        public static float fixedDeltaTime{
            get {
                return _fixedDeltaTime;
            }
        }

        private static float lastUpdateGameTime = 0;
        
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
            realtimeSinceStartup = totalSeconds;
            time += deltaTime;
        }

        internal static void InitializeDeltaTimeUpdate(float totalSeconds)
        {
            //actualDeltaTime = totalSeconds - lastUpdateGameTime;
            float newDeltaTime = actualDeltaTime * timeScale;
            lastUpdateGameTime = totalSeconds;
            //Debug.Display("newDeltaTime", newDeltaTime);
        }

        internal static void InitializeDeltaTimeFixedUpdate()
        {
            deltaTime = fixedDeltaTime * timeScale; //set delta time to constant rate when called from a FixedUpdate method
        }
    }
}
