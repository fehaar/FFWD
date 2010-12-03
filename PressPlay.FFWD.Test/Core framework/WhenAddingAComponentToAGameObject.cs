using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenAddingAComponentToAGameObject
    {
        [Test]
        public void ItWillHaveTheGameObjectSet()
        {
            GameObject go = new GameObject();
            Component comp = new TestComponent();
            go.AddComponent(comp);
            Assert.That(comp.gameObject, Is.SameAs(go));
        }
    }
}
