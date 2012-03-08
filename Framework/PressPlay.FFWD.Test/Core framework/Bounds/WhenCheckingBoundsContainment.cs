using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test
{
    [TestFixture]
    public class WhenCheckingBoundsContainment
    {
        Bounds b;

        [Test]
        public void BoundsCanContainAPoint()
        {
            b = new Bounds(Vector3.zero, Vector3.one);

            Assert.That(b.Contains(Vector3.zero));
        }

        [Test]
        public void BoundsCanNotContainAPoint()
        {
            b = new Bounds(Vector3.zero, Vector3.one);

            Assert.That(!b.Contains(new Vector3(2, 0, 0)));
        }

        [Test]
        public void IfAPointIntersectsTheBoundsItWillBeContained()
        {
            b = new Bounds(Vector3.zero, Vector3.one);

            Assert.That(b.Contains(new Vector3(0.5f, 0.5f, 0.5f)));
        }
	
	
	
    }
}
