using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenCreatingBounds
    {
        [Test]
        public void CreatingABoxWithCenterAndSizeWillSetTheCorrectProperties()
        {
            Vector3 c = new Vector3(2, 1, 5);
            Vector3 sz = new Vector3(4, 4, 4);
            Bounds b = new Bounds(c, sz);

            Assert.That(b.center, Is.EqualTo(c));
            Assert.That(b.size, Is.EqualTo(sz));
            Assert.That(b.extents, Is.EqualTo(sz / 2));
            Assert.That(b.max, Is.EqualTo(new Vector3(4, 3, 7)));
            Assert.That(b.min, Is.EqualTo(new Vector3(0, -1, 3)));
        }
	
    }
}
