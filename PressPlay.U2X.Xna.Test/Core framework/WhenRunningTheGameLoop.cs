using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PressPlay.U2X.Xna.Interfaces;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PressPlay.U2X.Xna.Test.Core_framework
{
    [TestFixture]
    public class WhenRunningTheGameLoop
    {
        Scene scene;
        GameObject go;
        TestComponent component;

        [SetUp]
        public void Setup()
        {
            scene = new Scene();
            go = new GameObject();
            component = new TestComponent();

            go.components.Add(component);
            scene.gameObjects.Add(go);
        }

        [Test]
        public void FixedUpdateWillNotGetCalledOnComponentsThatAreNotAwoken()
        {
            bool fixedUpdateCalled = false;
            component.onFixedUpdate = () => { fixedUpdateCalled = true; };
            Assert.That(fixedUpdateCalled, Is.False);
            scene.FixedUpdate();
            Assert.That(fixedUpdateCalled, Is.False);
        }
	
        [Test]
        public void FixedUpdateWillCallFixedUpdateOnComponentsOnTheScene()
        {
            bool fixedUpdateCalled = false;
            component.onFixedUpdate = () => { fixedUpdateCalled = true; };
            Component.AwakeNewComponents();

            Assert.That(fixedUpdateCalled, Is.False);
            scene.FixedUpdate();
            Assert.That(fixedUpdateCalled, Is.True);
        }

        [Test]
        public void UpdateWillNotGetCalledOnComponentsThatAreNotAwoken()
        {
            bool updateCalled = false;
            component.onUpdate = () => { updateCalled = true; };
            Assert.That(updateCalled, Is.False);
            scene.Update();
            Assert.That(updateCalled, Is.False);
        }

        [Test]
        public void UpdateWillCallUpdateOnComponentsOnTheScene()
        {
            bool updateCalled = false;
            component.onUpdate = () => { updateCalled = true; };
            Component.AwakeNewComponents();

            Assert.That(updateCalled, Is.False);
            scene.Update();
            Assert.That(updateCalled, Is.True);
        }

        [Test]
        public void DrawWillNotGetCalledOnComponentsThatAreNotAwoken()
        {
            bool drawCalled = false;
            component.onDraw = () => { drawCalled = true; };
            Assert.That(drawCalled, Is.False);
            scene.Draw(null);
            Assert.That(drawCalled, Is.False);
        }

        [Test]
        public void DrawWillCallFixedDrawOnComponentsOnTheScene()
        {
            bool drawCalled = false;
            component.onDraw = () => { drawCalled = true; };
            Component.AwakeNewComponents();

            Assert.That(drawCalled, Is.False);
            scene.Draw(null);
            Assert.That(drawCalled, Is.True);
        }

        [Test]
        public void StartIsCalledTheFirstTimeFixedUpdateIsCalled()
        {
            int startCalledCount = 0;
            component.onStart = () => { startCalledCount++; };
            Component.AwakeNewComponents();

            Assert.That(startCalledCount, Is.EqualTo(0));
            scene.FixedUpdate();
            Assert.That(startCalledCount, Is.EqualTo(1));
            scene.FixedUpdate();
            Assert.That(startCalledCount, Is.EqualTo(1));
        }

        [Test]
        public void StartIsCalledTheFirstTimeUpdateIsCalled()
        {
            int startCalledCount = 0;
            component.onStart = () => { startCalledCount++; };
            Component.AwakeNewComponents();

            Assert.That(startCalledCount, Is.EqualTo(0));
            scene.Update();
            Assert.That(startCalledCount, Is.EqualTo(1));
            scene.Update();
            Assert.That(startCalledCount, Is.EqualTo(1));
        }

        [Test]
        public void StartWillOnyBeCalledOnceInTheEntireLoop()
        {
            int startCalledCount = 0;
            component.onStart = () => { startCalledCount++; };
            Component.AwakeNewComponents();

            Assert.That(startCalledCount, Is.EqualTo(0));
            scene.FixedUpdate();
            Assert.That(startCalledCount, Is.EqualTo(1));
            scene.Update();
            Assert.That(startCalledCount, Is.EqualTo(1));
        }

    }
}
