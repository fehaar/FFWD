using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.GameObjects
{
    [TestFixture]
    public class WhenGettingComponents
    {
        TestHierarchy h;

        [SetUp]
        public void Setup()
        {
            h = new TestHierarchy();
        }

        [Test]
        public void WeCanGetAllComponentsInAnObjectsChildren()
        {
            Transform[] ts = h.root.GetComponentsInChildren<Transform>();

            Assert.That(ts, Is.Not.Null);
            Assert.That(ts.Length, Is.EqualTo(3));
        }

        [Test]
        public void WeWillOnlyGetComponentsFromActiveChildrenByDefault()
        {
            h.childOfChild.active = false;
            Transform[] ts = h.root.GetComponentsInChildren<Transform>();

            Assert.That(ts, Is.Not.Null);
            Assert.That(ts.Length, Is.EqualTo(2));
        }

        [Test]
        public void WeCanOptInToGetInactiveChildren()
        {
            h.childOfChild.active = false;
            Transform[] ts = h.root.GetComponentsInChildren<Transform>(true);

            Assert.That(ts, Is.Not.Null);
            Assert.That(ts.Length, Is.EqualTo(3));
        }
	
    }
}
