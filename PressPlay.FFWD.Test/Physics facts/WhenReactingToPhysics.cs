using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PressPlay.FFWD.Test.Core_framework;
using Box2D.XNA;

namespace PressPlay.FFWD.Test.Physics_facts
{
    [TestFixture]
    public class WhenReactingToPhysics
    {
        GameObject go;
        TestComponent component;
        TestComponent childComponent;

        [SetUp]
        public void Setup()
        {
            go = new GameObject();
            component = new TestComponent();
            go.AddComponent(component);

            GameObject child = new GameObject();
            childComponent = new TestComponent();
            child.AddComponent(childComponent);
            child.transform.parent = go.transform;
        }

        [TearDown]
        public void TearDown()
        {
            Application.AwakeNewComponents();
            Application.Reset();
        }

        #region OnTriggerEnter calls
		[Test]
        public void GameObjectWillNotCallOnTriggerEnterOnComponentsNotAwoken()
        {
            bool componentCalled = false;
            component.onTriggerEnter = () => componentCalled = true;
            go.OnTriggerEnter(null);
            Assert.That(componentCalled, Is.False);
        }

        [Test]
        public void GameObjectWillCallOnTriggerEnterOnAwokenComponents()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            component.onTriggerEnter = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnTriggerEnter(null);
            Assert.That(componentCalled, Is.True);
        }

        [Test]
        public void GameObjectWillCallOnTriggerEnterOnAwokenComponentsInChildObjects()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            childComponent.onTriggerEnter = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnTriggerEnter(null);
            Assert.That(componentCalled, Is.True);
        }
	    #endregion    

        #region OnTriggerExit calls
        [Test]
        public void GameObjectWillNotCallOnTriggerExitOnComponentsNotAwoken()
        {
            bool componentCalled = false;
            component.onTriggerExit = () => componentCalled = true;
            go.OnTriggerExit(null);
            Assert.That(componentCalled, Is.False);
        }

        [Test]
        public void GameObjectWillCallOnTriggerExitOnAwokenComponents()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            component.onTriggerExit = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnTriggerExit(null);
            Assert.That(componentCalled, Is.True);
        }

        [Test]
        public void GameObjectWillCallOnTriggerExitOnAwokenComponentsInChildObjects()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            childComponent.onTriggerExit = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnTriggerExit(null);
            Assert.That(componentCalled, Is.True);
        }
        #endregion    

        #region OnCollisionEnter calls
        [Test]
        public void GameObjectWillNotCallOnCollisionEnterOnComponentsNotAwoken()
        {
            bool componentCalled = false;
            component.onCollisionEnter = () => componentCalled = true;
            go.OnCollisionEnter(null);
            Assert.That(componentCalled, Is.False);
        }

        [Test]
        public void GameObjectWillCallOnCollisionEnterOnStartedComponents()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            component.onCollisionEnter = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnCollisionEnter(null);
            Assert.That(componentCalled, Is.True);
        }

        [Test]
        public void GameObjectWillCallOnCollisionEnterOnAwokenComponentsInChildObjects()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            childComponent.onCollisionEnter = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnCollisionEnter(null);
            Assert.That(componentCalled, Is.True);
        }
        #endregion

        #region OnCollisionExit calls
        [Test]
        public void GameObjectWillNotCallOnCollisionExitOnComponentsNotAwoken()
        {
            bool componentCalled = false;
            component.onCollisionExit = () => componentCalled = true;
            go.OnCollisionExit(null);
            Assert.That(componentCalled, Is.False);
        }

        [Test]
        public void GameObjectWillCallOnCollisionExitOnAwokenComponents()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            component.onCollisionExit = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnCollisionExit(null);
            Assert.That(componentCalled, Is.True);
        }

        [Test]
        public void GameObjectWillCallOnCollisionExitOnAwokenComponentsInChildObjects()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            childComponent.onCollisionExit = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnCollisionExit(null);
            Assert.That(componentCalled, Is.True);
        }
        #endregion

        #region OnPreSolve calls
        [Test]
        public void GameObjectWillNotCallOnPreSolveOnComponentsNotAwoken()
        {
            bool componentCalled = false;
            component.onPreSolve = () => componentCalled = true;
            go.OnPreSolve(null, new Manifold());
            Assert.That(componentCalled, Is.False);
        }

        [Test]
        public void GameObjectWillCallOnPreSolveOnAwokenComponents()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            component.onPreSolve = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnPreSolve(null, new Manifold());
            Assert.That(componentCalled, Is.True);
        }

        [Test]
        public void GameObjectWillCallOnPreSolveOnAwokenComponentsInChildObjects()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            childComponent.onPreSolve = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnPreSolve(null, new Manifold());
            Assert.That(componentCalled, Is.True);
        }
        #endregion

        #region OnPostSolve calls
        [Test]
        public void GameObjectWillNotCallOnPostSolveOnComponentsNotAwoken()
        {
            bool componentCalled = false;
            component.onPostSolve = () => componentCalled = true;
            go.OnPostSolve(null, new ContactImpulse());
            Assert.That(componentCalled, Is.False);
        }

        [Test]
        public void GameObjectWillCallOnPostSolveOnAwokenComponents()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            component.onPostSolve = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnPostSolve(null, new ContactImpulse());
            Assert.That(componentCalled, Is.True);
        }

        [Test]
        public void GameObjectWillCallOnPostSolveOnAwokenComponentsInChildObjects()
        {
            Assert.Inconclusive("It is not given that this should be implemented like this. Perhaps collision will only be called on components.");
            bool componentCalled = false;
            childComponent.onPostSolve = () => componentCalled = true;
            Application.AwakeNewComponents();
            go.OnPostSolve(null, new ContactImpulse());
            Assert.That(componentCalled, Is.True);
        }
        #endregion

    }
}
