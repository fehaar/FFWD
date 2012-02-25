using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenUsingDoNotDestroyOnLoad
    {
        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void TheGameObjectWillBeMarkedNotToBeDestroyedOnLevelLoad()
        {
            TestHierarchy h = new TestHierarchy();
            Assert.That(Application.dontDestroyOnLoad, !Contains.Item(h.childOfChild));
            UnityObject.DontDestroyOnLoad(h.childOfChild);

            Assert.That(Application.dontDestroyOnLoad, Contains.Item(h.childOfChild));
        }

        [Test]
        public void OnAComponentItsGameObjectWillBeMarkedNotToBeDestroyedOnLevelLoad()
        {
            TestHierarchy h = new TestHierarchy();
            TestComponent tc = new TestComponent();
            h.childOfChild.AddComponent(tc);

            Assert.That(Application.dontDestroyOnLoad, !Contains.Item(h.childOfChild));
            UnityObject.DontDestroyOnLoad(tc);

            Assert.That(Application.dontDestroyOnLoad, Contains.Item(h.childOfChild));
        }

        [Test]
        public void TheGameObjectsChildrenWillBeMarkedNotToBeDestroyedOnLevelLoad()
        {
            TestHierarchy h = new TestHierarchy();
            Assert.That(Application.dontDestroyOnLoad, !Contains.Item(h.root));
            Assert.That(Application.dontDestroyOnLoad, !Contains.Item(h.child));
            Assert.That(Application.dontDestroyOnLoad, !Contains.Item(h.childOfChild));
            UnityObject.DontDestroyOnLoad(h.root);

            Assert.That(Application.dontDestroyOnLoad, Contains.Item(h.root));
            Assert.That(Application.dontDestroyOnLoad, Contains.Item(h.child));
            Assert.That(Application.dontDestroyOnLoad, Contains.Item(h.childOfChild));
        }
    }
}
