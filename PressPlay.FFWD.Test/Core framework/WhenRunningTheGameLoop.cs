using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenRunningTheGameLoop
    {
        TestComponent component;
        TestComponent childComponent;
        TestComponent prefabComponent;
        Application app;

        [SetUp]
        public void Setup()
        {
            Scene scene = new Scene();
            GameObject go = new GameObject();
            component = new TestComponent();
            go.AddComponent(component);
            scene.gameObjects.Add(go);

            GameObject child = new GameObject();
            childComponent = new TestComponent();
            child.AddComponent(childComponent);
            child.transform.parent = go.transform;

            GameObject prefab = new GameObject();
            prefabComponent = new TestComponent();
            prefab.AddComponent(prefabComponent);
            scene.prefabs.Add(prefab);

            app = new Application(new Game());
            Application.LoadLevel(scene);
        }

        #region Fixed update
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
        public void PrefabComponentsWillNotBeFixedUpdated()
        {
            bool fixedUpdateCalled = false;
            prefabComponent.onFixedUpdate = () => { fixedUpdateCalled = true; };

            Assert.That(fixedUpdateCalled, Is.False);
            app.Update(new GameTime());
            Assert.That(fixedUpdateCalled, Is.False);
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
            bool updateCalled = false;
            prefabComponent.onUpdate = () => { updateCalled = true; };

            Assert.That(updateCalled, Is.False);
            app.Update(new GameTime());
            Assert.That(updateCalled, Is.False);
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

        #endregion

        #region Start calls
        [Test]
        public void StartIsCalledTheFirstTimeFixedUpdateIsCalled()
        {
            int startCalledCount = 0;
            component.onStart = () => { startCalledCount++; };

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

            Assert.That(startCalledCount, Is.EqualTo(0));
            app.Update(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
            app.Draw(new GameTime());
            Assert.That(startCalledCount, Is.EqualTo(1));
        }
        #endregion
    }
}
