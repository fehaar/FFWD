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
        public void WeWillGetTheTransformIfItIsThere()
        {
            Assert.That(comp.transform, Is.Null);

            Transform trans = new Transform();
            go.AddComponent(trans);

            Assert.That(comp.transform, Is.Not.Null);
            Assert.That(comp.transform, Is.SameAs(trans));
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
    }
}
