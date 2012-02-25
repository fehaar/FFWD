using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenSettingTheCenterOfBounds
    {
        [Test]
        public void TheCenterWillBeMoved()
        {
            Bounds b = new Bounds(Vector3.zero, Vector3.one);
            Vector3 newCenter = new Vector3(2, 2, 1);
            b.center = newCenter;

            Assert.That(b.center, Is.EqualTo(newCenter));
        }

        [Test]
        public void ExtentAndSizeWillRemainTheSame()
        {
            Bounds b = new Bounds(Vector3.zero, Vector3.one);
            Vector3 sz = b.size;
            Vector3 ext = b.extents;
            b.center = new Vector3(2, 2, 1);

            Assert.That(b.size, Is.EqualTo(sz));
            Assert.That(b.extents, Is.EqualTo(ext));
        }

        [Test]
        public void MinAndMaxWillBeMoved()
        {
            Bounds b = new Bounds(Vector3.zero, Vector3.one);
            Vector3 min = b.min;
            Vector3 max = b.max;
            Vector3 newCenter = new Vector3(2, 2, 1);
            b.center = newCenter;

            Assert.That(b.min, Is.EqualTo(min + newCenter));
            Assert.That(b.max, Is.EqualTo(max + newCenter));
        }
	
	
    }
}
