using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace PressPlay.U2X.Xna.Test.Physics_facts
{
    [TestFixture]
    public class WhenInitializingPhysics
    {
        [Test]
        public void TheWorldWillBeCreatedOnAccess()
        {
            Assert.That(Physics.world, Is.Not.Null);
        }

        [Test]
        public void WeCanSetTheInitialGravityByInitializingExplicitly()
        {
            Physics.Initialize(Vector2.One, null);
            Assert.That(Physics.world.Gravity, Is.EqualTo(Vector2.One));
        }
    }
}
