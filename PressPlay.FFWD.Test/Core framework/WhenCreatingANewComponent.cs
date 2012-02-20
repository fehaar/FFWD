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
        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void ItWillBeAwokenWhenAddedToTheGameObject()
        {
            bool isAwoken = false;
            GameObject go = new GameObject();
            TestComponent comp = new TestComponent() { onAwake = () => isAwoken = true };
            go.AddComponent(comp);
            Assert.That(isAwoken, Is.True);
        }

        [Test]
        public void ItWillOnlyGetAwakeCalledOnce()
        {
            int awakeCount = 0;
            GameObject go = new GameObject();
            TestComponent comp = go.AddComponent<TestComponent>();
            comp.onAwake = () => awakeCount++;
            Assert.That(awakeCount, Is.EqualTo(0));
            Application.AwakeNewComponents(null);
            Assert.That(awakeCount, Is.EqualTo(0));
        }

        [Test]
        public void WeCanFindTheNewComponentById()
        {
            GameObject go = new GameObject();
            TestComponent comp = go.AddComponent<TestComponent>();
            Assert.That(Application.Find(comp.GetInstanceID()), Is.Not.Null);
            Assert.That(Application.Find(comp.GetInstanceID()), Is.SameAs(comp));
        }

        [Test]
        public void WeCanRemoveComponentsByResettingTheApplication()
        {
            GameObject go = new GameObject();
            TestComponent comp = go.AddComponent<TestComponent>();
            Application.Reset();
            Assert.That(Application.Find(comp.GetInstanceID()), Is.Null);
        }
	
    }
}
