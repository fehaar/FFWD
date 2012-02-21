using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenUsingTheComponentShortcutProperties
    {
        GameObject go;
        TestComponent comp;

        [SetUp]
        public void SetUp()
        {
            go = new GameObject();
            comp = go.AddComponent(new TestComponent());            
        }

        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void IfTheComponentHasNoGameObjectTransformWillBeNull()
        {
            TestComponent comp = new TestComponent();
            Assert.That(comp.transform, Is.Null);
        }

        [Test]
        public void WeWillGetTheRigidBodyIfItIsThere()
        {
            Assert.That(comp.rigidbody, Is.Null);

            Rigidbody body = new Rigidbody();
            go.AddComponent(body);

            Assert.That(comp.rigidbody, Is.Not.Null);
            Assert.That(comp.rigidbody, Is.SameAs(body));
        }

        [Test]
        public void WeWillGetTheCorrectRigidbodyAfterAnInstantiation()
        {
            Rigidbody body = new Rigidbody();
            go.AddComponent(body);
            Assert.That(comp.rigidbody, Is.Not.Null);

            TestComponent inst = (TestComponent)GameObject.Instantiate(comp);
            Assert.That(inst.rigidbody, Is.Not.Null);
            Assert.That(inst.rigidbody, Is.Not.SameAs(body));
        }

        [Test]
        public void IfTheComponentHasNoGameObjectRigidbodyWillBeNull()
        {
            TestComponent comp = new TestComponent();
            Assert.That(comp.rigidbody, Is.Null);
        }

        [Test]
        public void WeWillGetTheColliderIfItIsThere()
        {
            Assert.That(comp.collider, Is.Null);

            Collider body = new BoxCollider();
            go.AddComponent(body);

            Assert.That(comp.collider, Is.Not.Null);
            Assert.That(comp.collider, Is.SameAs(body));
        }

        [Test]
        public void WeWillGetTheCorrectColliderAfterAnInstantiation()
        {
            Collider body = new BoxCollider();
            go.AddComponent(body);
            Assert.That(comp.collider, Is.Not.Null);

            TestComponent inst = (TestComponent)GameObject.Instantiate(comp);
            Assert.That(inst.collider, Is.Not.Null);
            Assert.That(inst.collider, Is.Not.SameAs(body));
        }


        [Test]
        public void IfTheComponentHasNoGameObjectColliderWillBeNull()
        {
            TestComponent comp = new TestComponent();
            Assert.That(comp.collider, Is.Null);
        }
    }
}
