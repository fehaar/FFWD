using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenSettingActiveOnAGameObject
    {
        [Test]
        public void TheChildrenWillNotBeAffected()
        {
            TestHierarchy h = new TestHierarchy();
            Assert.That(h.root.active, Is.True);
            Assert.That(h.child.active, Is.True);
            Assert.That(h.childOfChild.active, Is.True);

            h.root.active = false;
            Assert.That(h.root.active, Is.False);
            Assert.That(h.child.active, Is.True);
            Assert.That(h.childOfChild.active, Is.True);
        }

        [Test]
        public void TheChildrenWillBeSetIfSettingRecursively()
        {
            TestHierarchy h = new TestHierarchy();
            Assert.That(h.root.active, Is.True);
            Assert.That(h.child.active, Is.True);
            Assert.That(h.childOfChild.active, Is.True);

            h.root.SetActiveRecursively(false);
            Assert.That(h.root.active, Is.False);
            Assert.That(h.child.active, Is.False);
            Assert.That(h.childOfChild.active, Is.False);
        }
	
    }
}
