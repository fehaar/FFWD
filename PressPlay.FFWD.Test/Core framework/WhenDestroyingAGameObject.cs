using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenDestroyingAGameObject
    {
        [Test]
        public void ItWillBeMarkedForDestruction()
        {
            GameObject go = new GameObject();

            UnityObject.Destroy(go);

            Assert.That(Application.markedForDestruction.Contains(go));
        }

        [Test]
        public void ItWillBeRemovedFromExistanceWhenTheApplicationCleansUp()
        {
            GameObject go = new GameObject();
            Application.AwakeNewComponents();
            Assert.That((bool)go, Is.True);

            UnityObject.Destroy(go);

            Application.CleanUp();

            Assert.That((bool)go, Is.False);
        }  

    }
}
