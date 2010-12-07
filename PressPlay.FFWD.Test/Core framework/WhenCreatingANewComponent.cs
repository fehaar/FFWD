using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenCreatingANewComponent
    {
        [Test]
        public void ItWillBeAwokenOnTheNextAwakeNewComponentsCall()
        {
            bool isAwoken = false;
            TestComponent comp = new TestComponent();
            comp.onAwake = () => isAwoken = true;
            Assert.That(isAwoken, Is.False);
            Application.AwakeNewComponents();
            Assert.That(isAwoken, Is.True);
        }

        [Test]
        public void ItWillOnlyGetAwakeCalledOnce()
        {
            int awakeCount = 0;
            TestComponent comp = new TestComponent();
            comp.onAwake = () => awakeCount++;
            Assert.That(awakeCount, Is.EqualTo(0));
            Application.AwakeNewComponents();
            Assert.That(awakeCount, Is.EqualTo(1));
            Application.AwakeNewComponents();
            Assert.That(awakeCount, Is.EqualTo(1));
        }
    }
}
