using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
	[TestFixture]
	public class WhenConvertingAColor
	{
		[Test]
		public void WeWillGetTheSameColorIfWeHaveFullAlpha()
		{
            Color c = new Color(0.5f, 0.5f, 0.5f);
            Microsoft.Xna.Framework.Color cl = c;
            Assert.That(cl.R, Is.EqualTo(128));
            Assert.That(cl.G, Is.EqualTo(128));
            Assert.That(cl.B, Is.EqualTo(128));
        }

        [Test]
        public void WeCanParseTheOutputOfToString()
        {
            Color c = new Color(0.5f, 0.5f, 0.5f);
            Color c1 = Color.Parse(c.ToString());

            Assert.That(c1.R, Is.EqualTo(c.R));
            Assert.That(c1.G, Is.EqualTo(c.G));
            Assert.That(c1.B, Is.EqualTo(c.B));
            Assert.That(c1.A, Is.EqualTo(c.A));
        }
	
	}
}
