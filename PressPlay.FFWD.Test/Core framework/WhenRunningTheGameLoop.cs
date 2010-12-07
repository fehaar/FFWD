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
        Application app;

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
            app = new Application(new Game());
            Application.LoadLevel(scene);
        }

        #region Fixed update
        //[Test]
        //public void FixedUpdateWillAwakeNewComponents()
        //{
        //    Assert.That(Application.IsAwake(component), Is.False);
        //    app.Update(new GameTime());
        //    Assert.That(Application.IsAwake(component), Is.True);
        //}

        [Test]
        public void FixedUpdateWillCallFixedUpdateOnComponentsOnTheScene()
        {
            bool fixedUpdateCalled = false;
            component.onFixedUpdate = () => { fixedUpdateCalled = true; };

            Assert.That(fixedUpdateCalled, Is.False);
            app.Update(new GameTime());
            Assert.That(fixedUpdateCalled, Is.True);
        }


        [Test]
        public void FixedUpdateWillCallFixedUpdateOnChildComponentsOnTheScene()
        {
            bool fixedUpdateCalled = false;
            childComponent.onFixedUpdate = () => { fixedUpdateCalled = true; };

            Assert.That(fixedUpdateCalled, Is.False);
            app.Update(new GameTime());
            Assert.That(fixedUpdateCalled, Is.True);
        }

        [Test]
        public void FixedUpdateWillNotBeCalledOnPrefabComponents()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");
        }
        #endregion

        #region Update calls
        //[Test]
        //public void UpdateWillNotGetCalledOnComponentsThatAreNotAwoken()
        //{
        //    bool updateCalled = false;
        //    component.onUpdate = () => { updateCalled = true; };
        //    Assert.That(updateCalled, Is.False);
        //    app.Draw(new GameTime());
        //    Assert.That(updateCalled, Is.False);
        //}

        //[Test]
        //public void UpdateWillNotGetCalledOnChildComponentsThatAreNotAwoken()
        //{
        //    bool updateCalled = false;
        //    childComponent.onUpdate = () => { updateCalled = true; };
        //    Assert.That(updateCalled, Is.False);
        //    app.Draw(new GameTime());
        //    Assert.That(updateCalled, Is.False);
        //}

        [Test]
        public void UpdateWillCallUpdateOnComponentsOnTheScene()
        {
            bool updateCalled = false;
            component.onUpdate = () => { updateCalled = true; };
            Application.AwakeNewComponents();

            Assert.That(updateCalled, Is.False);
            app.Draw(new GameTime());
            Assert.That(updateCalled, Is.True);
        }

        [Test]
        public void UpdateWillCallUpdateOnChildComponentsOnTheScene()
        {
            bool updateCalled = false;
            childComponent.onUpdate = () => { updateCalled = true; };
            Application.AwakeNewComponents();

            Assert.That(updateCalled, Is.False);
            app.Draw(new GameTime());
            Assert.That(updateCalled, Is.True);
        }

        [Test]
        public void UpdateWillNotBeCalledOnPrefabComponents()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");
        }
        #endregion

        #region Draw calls
        //[Test]
        //public void DrawWillNotGetCalledOnComponentsThatAreNotAwoken()
        //{
        //    bool drawCalled = false;
        //    component.onDraw = () => { drawCalled = true; };
        //    Assert.That(drawCalled, Is.False);
        //    app.Draw(new GameTime());
        //    Assert.That(drawCalled, Is.False);
        //}

        //[Test]
        //public void DrawWillNotGetCalledOnChildComponentsThatAreNotAwoken()
        //{
        //    bool drawCalled = false;
        //    childComponent.onDraw = () => { drawCalled = true; };
        //    Assert.That(drawCalled, Is.False);
        //    app.Draw(new GameTime());
        //    Assert.That(drawCalled, Is.False);
        //}

        [Test]
        public void DrawWillCallDrawOnComponentsOnTheScene()
        {
            bool drawCalled = false;
            component.onDraw = () => { drawCalled = true; };
            Application.AwakeNewComponents();

            Assert.That(drawCalled, Is.False);
            app.Draw(new GameTime());
            Assert.That(drawCalled, Is.True);
        }

        [Test]
        public void DrawWillCallDrawOnChildComponentsOnTheScene()
        {
            bool drawCalled = false;
            childComponent.onDraw = () => { drawCalled = true; };
            Application.AwakeNewComponents();

            Assert.That(drawCalled, Is.False);
            app.Draw(new GameTime());
            Assert.That(drawCalled, Is.True);
        }

        [Test]
        public void DrawWillNotBeCalledOnPrefabComponents()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");
        }
        #endregion

        #region Start calls
        [Test]
        public void StartIsCalledTheFirstTimeFixedUpdateIsCalled()
        {
            int startCalledCount = 0;
            component.onStart = () => { startCalledCount++; };
            Application.AwakeNewComponents();

            Assert.That(startCalledCount, Is.EqualTo(0));
            app.Update(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
            app.Update(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
        }

        [Test]
        public void StartIsCalledTheFirstTimeUpdateIsCalled()
        {
            int startCalledCount = 0;
            component.onStart = () => { startCalledCount++; };
            Application.AwakeNewComponents();

            Assert.That(startCalledCount, Is.EqualTo(0));
            app.Draw(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
            app.Draw(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
        }

        [Test]
        public void StartWillOnyBeCalledOnceInTheEntireLoop()
        {
            int startCalledCount = 0;
            component.onStart = () => { startCalledCount++; };
            Application.AwakeNewComponents();

            Assert.That(startCalledCount, Is.EqualTo(0));
            app.Update(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
            app.Draw(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
        }
        #endregion
    }
}
