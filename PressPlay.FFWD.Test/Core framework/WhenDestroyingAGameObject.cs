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
        [TearDown]
        public void TearDown( )
        {
            Application.Reset();
        }

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
            Application.AwakeNewComponents(false);
            Assert.That((bool)go, Is.True);

            UnityObject.Destroy(go);

            Application.CleanUp();

            Assert.That((bool)go, Is.False);
        }

        [Test]
        public void AllItsComponentsWillBeMarkedForDestruction()
        {
            GameObject go = new GameObject();
            Transform t = go.transform;

            UnityObject.Destroy(go);

            Assert.That(Application.markedForDestruction.Contains(t));
        }


        [Test]
        public void AllOfItsChildrenWillBeMarkedForDestruction()
        {
            TestHierarchy h = new TestHierarchy();

            UnityObject.Destroy(h.root);

            Assert.That(Application.markedForDestruction.Contains(h.child));
            Assert.That(Application.markedForDestruction.Contains(h.childOfChild));
        }

    }
}
