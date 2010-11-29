using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD
{
    public class Time : DrawableGameComponent
    {
        public Time(Game game)
            : base(game)
        {
            UpdateOrder = 0;
        }

        public static float time = 0.0f;
        public static float deltaTime = 0.0f;
        public static float actualDeltaTime = 0.0f;
        public static float timeScale = 1.0f;
        public static float realtimeSinceStartup = 0.0f;
        public static float timeSinceLastDraw = 0.0f;
        public static TimeSpan frameTimeSpan;
        private float lastDrawCall = 0.0f;

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            timeSinceLastDraw = time - lastDrawCall;
            lastDrawCall = time;
        }

        public override void Update(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds * timeScale;
            actualDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            realtimeSinceStartup = (float)gameTime.TotalGameTime.TotalSeconds;
            time += deltaTime;
            frameTimeSpan = gameTime.ElapsedGameTime;
            base.Update(gameTime);
        }
    }
}
