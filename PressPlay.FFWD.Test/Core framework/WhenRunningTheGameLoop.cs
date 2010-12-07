using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PressPlay.FFWD.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenRunningTheGameLoop
    {
        TestComponent component;
        TestComponent childComponent;

        [SetUp]
        public void Setup()
        {
            Scene scene = new Scene();
            GameObject go = new GameObject();
            go.AddComponent(new Transform());
            component = new TestComponent();
            go.AddComponent(component);
            scene.gameObjects.Add(go);

            GameObject child = new GameObject();
            child.AddComponent(new Transform());
            childComponent = new TestComponent();
            child.AddComponent(childComponent);
            child.transform.parent = go.transform;
            new Application(new Game());
            Application.LoadScene(scene);
        }

        #region Fixed update
        [Test]
        public void FixedUpdateWillAwakeNewComponents()
        {
            Assert.That(Component.IsAwake(component), Is.False);
            Application.Instance.Update(new GameTime());
            Assert.That(Component.IsAwake(component), Is.True);
        }

        [Test]
        public void FixedUpdateWillCallFixedUpdateOnComponentsOnTheScene()
        {
            bool fixedUpdateCalled = false;
            component.onFixedUpdate = () => { fixedUpdateCalled = true; };

            Assert.That(fixedUpdateCalled, Is.False);
            Application.Instance.Update(new GameTime());
            Assert.That(fixedUpdateCalled, Is.True);
        }


        [Test]
        public void FixedUpdateWillCallFixedUpdateOnChildComponentsOnTheScene()
        {
            bool fixedUpdateCalled = false;
            childComponent.onFixedUpdate = () => { fixedUpdateCalled = true; };

            Assert.That(fixedUpdateCalled, Is.False);
            Application.Instance.Update(new GameTime());
            Assert.That(fixedUpdateCalled, Is.True);
        }
        #endregion

        #region Update calls
        [Test]
        public void UpdateWillNotGetCalledOnComponentsThatAreNotAwoken()
        {
            bool updateCalled = false;
            component.onUpdate = () => { updateCalled = true; };
            Assert.That(updateCalled, Is.False);
            Application.Instance.Draw(new GameTime());
            Assert.That(updateCalled, Is.False);
        }

        [Test]
        public void UpdateWillNotGetCalledOnChildComponentsThatAreNotAwoken()
        {
            bool updateCalled = false;
            childComponent.onUpdate = () => { updateCalled = true; };
            Assert.That(updateCalled, Is.False);
            Application.Instance.Draw(new GameTime());
            Assert.That(updateCalled, Is.False);
        }

        [Test]
        public void UpdateWillCallUpdateOnComponentsOnTheScene()
        {
            bool updateCalled = false;
            component.onUpdate = () => { updateCalled = true; };
            Component.AwakeNewComponents();

            Assert.That(updateCalled, Is.False);
            Application.Instance.Draw(new GameTime());
            Assert.That(updateCalled, Is.True);
        }

        [Test]
        public void UpdateWillCallUpdateOnChildComponentsOnTheScene()
        {
            bool updateCalled = false;
            childComponent.onUpdate = () => { updateCalled = true; };
            Component.AwakeNewComponents();

            Assert.That(updateCalled, Is.False);
            Application.Instance.Draw(new GameTime());
            Assert.That(updateCalled, Is.True);
        }
        #endregion

        #region Draw calls
        [Test]
        public void DrawWillNotGetCalledOnComponentsThatAreNotAwoken()
        {
            bool drawCalled = false;
            component.onDraw = () => { drawCalled = true; };
            Assert.That(drawCalled, Is.False);
            Application.Instance.Draw(new GameTime());
            Assert.That(drawCalled, Is.False);
        }

        [Test]
        public void DrawWillNotGetCalledOnChildComponentsThatAreNotAwoken()
        {
            bool drawCalled = false;
            childComponent.onDraw = () => { drawCalled = true; };
            Assert.That(drawCalled, Is.False);
            Application.Instance.Draw(new GameTime());
            Assert.That(drawCalled, Is.False);
        }

        [Test]
        public void DrawWillCallDrawOnComponentsOnTheScene()
        {
            bool drawCalled = false;
            component.onDraw = () => { drawCalled = true; };
            Component.AwakeNewComponents();

            Assert.That(drawCalled, Is.False);
            Application.Instance.Draw(new GameTime());
            Assert.That(drawCalled, Is.True);
        }

        [Test]
        public void DrawWillCallDrawOnChildComponentsOnTheScene()
        {
            bool drawCalled = false;
            childComponent.onDraw = () => { drawCalled = true; };
            Component.AwakeNewComponents();

            Assert.That(drawCalled, Is.False);
            Application.Instance.Draw(new GameTime());
            Assert.That(drawCalled, Is.True);
        }
        #endregion

        #region Start calls
        [Test]
        public void StartIsCalledTheFirstTimeFixedUpdateIsCalled()
        {
            int startCalledCount = 0;
            component.onStart = () => { startCalledCount++; };
            Component.AwakeNewComponents();

            Assert.That(startCalledCount, Is.EqualTo(0));
            Application.Instance.Update(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
            Application.Instance.Update(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
        }

        [Test]
        public void StartIsCalledTheFirstTimeUpdateIsCalled()
        {
            int startCalledCount = 0;
            component.onStart = () => { startCalledCount++; };
            Component.AwakeNewComponents();

            Assert.That(startCalledCount, Is.EqualTo(0));
            Application.Instance.Draw(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
            Application.Instance.Draw(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
        }

        [Test]
        public void StartWillOnyBeCalledOnceInTheEntireLoop()
        {
            int startCalledCount = 0;
            component.onStart = () => { startCalledCount++; };
            Component.AwakeNewComponents();

            Assert.That(startCalledCount, Is.EqualTo(0));
            Application.Instance.Update(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
            Application.Instance.Draw(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
        }
        #endregion
    }
}
