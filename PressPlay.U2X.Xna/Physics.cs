using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;

namespace PressPlay.U2X.Xna
{
    public static class Physics
    {
        internal static World world;
        private static bool isPaused = false;

        public static void Initialize(Vector2 gravity = new Vector2())
        {
            world = new World(gravity, true);
            world.ContactListener = new PhysicsContactListener();
            world.ContinuousPhysics = true;
        }

        public static void TogglePause()
        {
            isPaused = !isPaused;
        }

        public static void Update()
        {
            //if (!isPaused)
            //    world.Step((float)Time.deltaTime, 6, 4); // was world.Step((float)Time.deltaTime,6, 3); before precert..

            //// process Contacts
            //ProcessContacts();
        }
    }
}
