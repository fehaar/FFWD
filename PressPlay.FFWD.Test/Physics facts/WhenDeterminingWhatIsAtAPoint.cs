using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using Box2D.XNA;

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
            Body body = Physics.AddBody();
            Physics.AddBox(body, false, 10, 10, new Vector2(0, 50), 0, 1);
            bool hit = Physics.Pointcast(new Vector2(5, 55));
            Assert.That(hit, Is.True);
        }

        [Test]
        public void WeWillGetNoHitsIfThereIsNoFixturesAtThePoint()
        {
            Body body = Physics.AddBody();
            Physics.AddBox(body, false, 10, 10, new Vector2(0, 50), 0, 1);
            bool hit = Physics.Pointcast(new Vector2(-5, -5));
            Assert.That(hit, Is.False);
        }

        [Test]
        public void WeWillGetNoHitsIfThereIsNoFixtures()
        {
            bool hit = Physics.Pointcast(new Vector2(0, 0));
            Assert.That(hit, Is.False);
        }

        [Test]
        public void WeCanGetAGameObjectIfItIsAtTheCorrectPosition()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");
        }
	
    }
}
