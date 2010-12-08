using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenFindingAnObjectById
    {
        [TearDown]
        public void TearDown()
        {
            Application.AwakeNewComponents();
            Application.Reset();
        }

        [Test]
        public void WeCanFindAGameObject()
        {
            GameObject obj = new GameObject();

            UnityObject found = obj.GetObjectById(obj.GetInstanceID());

            Assert.That(found, Is.Not.Null);
            Assert.That(found, Is.SameAs(obj));
        }

        [Test]
        public void WeCanFindAComponentOfAGameObject()
        {
            TestHierarchy h = new TestHierarchy();

            UnityObject found = h.root.GetObjectById(h.rootTrans.GetInstanceID());

            Assert.That(found, Is.Not.Null);
            Assert.That(found, Is.SameAs(h.rootTrans));
        }

        [Test]
        public void WeCanFindAChildObject()
        {
            TestHierarchy h = new TestHierarchy();

            UnityObject found = h.root.GetObjectById(h.child.GetInstanceID());

            Assert.That(found, Is.Not.Null);
            Assert.That(found, Is.SameAs(h.child));
        }

        [Test]
        public void WeCanFindAChildComponent()
        {
            TestHierarchy h = new TestHierarchy();

            UnityObject found = h.root.GetObjectById(h.childTrans.GetInstanceID());

            Assert.That(found, Is.Not.Null);
            Assert.That(found, Is.SameAs(h.childTrans));
        }

        [Test]
        public void WeCanFindAChildOfChildObject()
        {
            TestHierarchy h = new TestHierarchy();

            UnityObject found = h.root.GetObjectById(h.childOfChild.GetInstanceID());

            Assert.That(found, Is.Not.Null);
            Assert.That(found, Is.SameAs(h.childOfChild));
        }

        [Test]
        public void WeCanFindAChildOfChildComponent()
        {
            TestHierarchy h = new TestHierarchy();

            UnityObject found = h.root.GetObjectById(h.childOfChildTrans.GetInstanceID());

            Assert.That(found, Is.Not.Null);
            Assert.That(found, Is.SameAs(h.childOfChildTrans));
        }
    }
}
