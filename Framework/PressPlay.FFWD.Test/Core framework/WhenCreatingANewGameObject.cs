using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenCreatingANewGameObject
    {
        [Test]
        public void ItWillAutomaticallyHaveATransform()
        {
            GameObject go = new GameObject();
            Assert.That(go.transform, Is.Not.Null);
        }

        [Test]
        public void WeCannotAddANewTransformToIt()
        {
            GameObject go = new GameObject();
            Assert.Throws<InvalidOperationException>(() => go.AddComponent(new Transform()));
        }
    }
}
