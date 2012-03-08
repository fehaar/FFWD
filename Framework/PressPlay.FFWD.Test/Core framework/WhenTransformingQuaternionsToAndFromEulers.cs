using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

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

        [Test]
        public void WhenSettingTheQuaternionWeGetTheSameEulerResultAsInUnity()
        {
            Quaternion[] qs = new Quaternion[] { 
                new Quaternion(0, 0.70179f, 0, 0.71238f),
                new Quaternion(0, 0.71349f, 0, -0.70067f)
            };
            Vector3[] expected = new Vector3[] { 
                new Vector3(0, 89.14144f, 0),
                new Vector3(0, 268.9612f, 0)
            };
            int errors = 0;
            for (int i = 0; i < qs.Length; i++)
            {
                Vector3 euler = qs[i].eulerAngles;
                Vector3 diff = expected[i] - euler;
                if (diff.sqrMagnitude > 0.00001f)
                {
                    errors++;
                }
            }
            Assert.That(errors, Is.EqualTo(0));
        }
    }
}
