using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenTransformingQuaternionsToAndFromEulers
    {
        [Test]
        public void WeGetTheSameResultWhenTransformingBack()
        {
            Vector3 rot = new Vector3(10, 20, 30);
            Quaternion q = Quaternion.Euler(rot);
            Vector3 euler = q.eulerAngles;

            Vector3 diff = rot - euler;
            Assert.That(diff.sqrMagnitude, Is.LessThan(0.00001f));
        }
    }
}
