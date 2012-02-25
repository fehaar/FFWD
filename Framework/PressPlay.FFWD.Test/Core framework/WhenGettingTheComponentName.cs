using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenGettingTheComponentName
    {
        [Test]
        public void ItWillBeTheSameAsTheGameObjectName()
        {
            GameObject go = new GameObject() { name = "My fab component" };
            Component comp = new TestComponent();
            go.AddComponent(comp);
            Assert.That(comp.name, Is.EqualTo(go.name));
        }

        [Test]
        public void ItWillBeTheSameAsTheTypeIfItIsNotAddedToAGameObject()
        {
            Component comp = new TestComponent();
            Assert.That(comp.name, Is.EqualTo(comp.GetType().Name));
        }
    }
}
