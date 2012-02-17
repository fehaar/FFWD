using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenDestroyingAComponent
    {
        [Test]
        public void ItWillBeRemovedFromTheGameObject()
        {
            GameObject go = new GameObject();
            TestComponent cmp = go.AddComponent(new TestComponent());

            Component.Destroy(cmp);

            Assert.That(cmp.gameObject, Is.Null);
            Assert.That(go.GetComponent<TestComponent>(), Is.Null);
        }

        [Test]
        public void ItWillBeMarkedForDestruction()
        {
            GameObject go = new GameObject();
            TestComponent cmp = go.AddComponent(new TestComponent());

            Component.Destroy(cmp);

            Assert.That(Application.markedForDestruction.Contains(cmp));
        }

        [Test]
        public void ItWillBeRemovedFromExistanceWhenTheApplicationCleansUp()
        {
            GameObject go = new GameObject();
            TestComponent cmp = go.AddComponent(new TestComponent());
            Application.AwakeNewComponents(false);
            Assert.That((bool)cmp, Is.True);

            Component.Destroy(cmp);

            Application.CleanUp();

            Assert.That((bool)cmp, Is.False);
        }  
	
    }
}
