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
	}
}
