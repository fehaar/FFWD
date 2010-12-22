using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Test.Physics_facts
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
            Physics.Initialize(PressPlay.FFWD.Vector2.one, null);
            Assert.That(Physics.world.Gravity, Is.EqualTo(PressPlay.FFWD.Vector2.one));
        }
    }
}
