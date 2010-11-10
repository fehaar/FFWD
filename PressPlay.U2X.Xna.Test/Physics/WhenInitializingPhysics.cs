using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace PressPlay.U2X.Xna.Test.Physics
{
    [TestFixture]
    public class WhenInitializingPhysics
    {
        [Test]
        public void InitializeWillCreateTheWorld()
        {
            Assert.That(Xna.Physics.world, Is.Null);
            Xna.Physics.Initialize();
            Assert.That(Xna.Physics.world, Is.Not.Null);
        }

        [Test]
        public void WeCanSetTheInitialGravity()
        {
            Xna.Physics.Initialize(Vector2.One);
            Assert.That(Xna.Physics.world.Gravity, Is.EqualTo(Vector2.One));
        }
    }
}
