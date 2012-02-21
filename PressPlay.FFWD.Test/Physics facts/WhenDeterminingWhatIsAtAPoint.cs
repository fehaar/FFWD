using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace PressPlay.FFWD.Test.Physics_facts
{
    [TestFixture]
    public class WhenDeterminingWhatIsAtAPoint
    {
        [SetUp]
        public void Setup()
        {
            Physics.Initialize();
        }

        [Test]
        public void WeWillGetAHitIfAFixtureIsThere()
        {
            Assert.Inconclusive("This test is suspended as we need an active collider on the body to test on");
            Body body = Physics.AddBody();
            Physics.AddBox(body, false, 10, 10, new Vector2(0, 50), 1);
            bool hit = Physics.Pointcast(new Vector2(5, 55));
            Assert.That(hit, Is.True);
        }

        [Test]
        public void WeWillGetNoHitsIfThereIsNoFixturesAtThePoint()
        {
            Assert.Inconclusive("This test is suspended as we need an active collider on the body to test on");
            Body body = Physics.AddBody();
            Physics.AddBox(body, false, 10, 10, new Vector2(0, 50), 1);
            bool hit = Physics.Pointcast(new Vector2(-5, -5));
            Assert.That(hit, Is.False);
        }

        [Test]
        public void WeWillGetNoHitsIfThereIsNoFixtures()
        {
            bool hit = Physics.Pointcast(new Vector2(0, 0));
            Assert.That(hit, Is.False);
        }
    }
}
