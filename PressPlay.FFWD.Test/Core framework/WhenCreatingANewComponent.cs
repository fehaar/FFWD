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

        [TearDown]
        public void TearDown()
        {
            Application.AwakeNewComponents();
            Application.Reset();
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

        [Test]
        public void WeCanFindTheNewComponentByIdAfterItHasBeenAwoken()
        {
            TestComponent comp = new TestComponent();

            Assert.That(Application.Find(comp.GetInstanceID()), Is.Null);
            Application.AwakeNewComponents();
            Assert.That(Application.Find(comp.GetInstanceID()), Is.Not.Null);
            Assert.That(Application.Find(comp.GetInstanceID()), Is.SameAs(comp));
        }

        [Test]
        public void WeCanRemoveComponentsByResettingTheApplication()
        {
            TestComponent comp = new TestComponent();

            Application.AwakeNewComponents();
            Assert.That(Application.Find(comp.GetInstanceID()), Is.Not.Null);
            Application.Reset();
            Assert.That(Application.Find(comp.GetInstanceID()), Is.Null);
        }
	
    }
}
