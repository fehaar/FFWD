using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenLoadingAScene
    {
        Scene scene;
        GameObject root;
        GameObject child;
        GameObject childOfChild;
        TestComponent rootComponent;
        TestComponent childComponent;
        TestComponent childOfChildComponent;

        [SetUp]
        public void Setup()
        {
            Application.AwakeNewComponents();
            Application.Reset();

            scene = new Scene();
            root = new GameObject();
            root.AddComponent(new Transform());
            rootComponent = new TestComponent();

            root.AddComponent(rootComponent);
            scene.gameObjects.Add(root);

            child = new GameObject();
            child.AddComponent(new Transform());
            childComponent = new TestComponent();
            child.AddComponent(childComponent);
            root.transform.children = new List<GameObject>();
            root.transform.children.Add(child);

            childOfChild = new GameObject();
            childOfChild.AddComponent(new Transform());
            childOfChildComponent = new TestComponent();
            childOfChild.AddComponent(childOfChildComponent);
            child.transform.children = new List<GameObject>();
            child.transform.children.Add(childOfChild);
        }

        [Test]
        public void AfterLoadWillEnsureThatAllReferencesAreSetOnComponents()
        {
            Assert.Inconclusive("This will only work when loading with the intermediate serializer, so we need to do a test like that.");
            Assert.That(rootComponent.gameObject, Is.Null);
            scene.AfterLoad();
            Assert.That(rootComponent.gameObject, Is.Not.Null);
            Assert.That(rootComponent.gameObject, Is.EqualTo(root));
        }

        [Test]
        public void AfterLoadWillEnsureThatAllReferencesAreSetOnComponentsAllTheWayThroughTheHierachy()
        {
            Assert.Inconclusive("This will only work when loading with the intermediate serializer, so we need to do a test like that.");
            Assert.That(childComponent.gameObject, Is.Null);
            Assert.That(childOfChildComponent.gameObject, Is.Null);
            scene.AfterLoad();
            Assert.That(childComponent.gameObject, Is.Not.Null);
            Assert.That(childComponent.gameObject, Is.EqualTo(child));
            Assert.That(childOfChildComponent.gameObject, Is.Not.Null);
            Assert.That(childOfChildComponent.gameObject, Is.EqualTo(childOfChild));
        }

        [Test]
        public void AfterLoadWillEnsureThatAllTransformParentReferencesAreSet()
        {
            Assert.That(child.transform.parent, Is.Null);
            scene.AfterLoad();
            Assert.That(child.transform.parent, Is.Not.Null);
            Assert.That(child.transform.parent, Is.SameAs(root.transform));
        }

        [Test]
        public void AfterLoadWillEnsureThatAllTransformParentReferencesAreSetAllTheWayThroughTheHierachy()
        {
            Assert.That(childOfChild.transform.parent, Is.Null);
            scene.AfterLoad();
            Assert.That(childOfChild.transform.parent, Is.Not.Null);
            Assert.That(childOfChild.transform.parent, Is.SameAs(child.transform));
        }

        [Test]
        public void AfterLoadWillEnsureThatAllTransformParentReferencesAreSetOnPrefabs()
        {
            scene.prefabs.AddRange(scene.gameObjects);
            scene.gameObjects.Clear();
            Assert.That(child.transform.parent, Is.Null);
            scene.AfterLoad();
            Assert.That(child.transform.parent, Is.Not.Null);
            Assert.That(child.transform.parent, Is.SameAs(root.transform));
        }

        [Test]
        public void AfterLoadWillEnsureThatAllTransformParentReferencesAreSetAllTheWayThroughTheHierachyOnPrefabs()
        {
            scene.prefabs.AddRange(scene.gameObjects);
            scene.gameObjects.Clear();
            Assert.That(childOfChild.transform.parent, Is.Null);
            scene.AfterLoad();
            Assert.That(childOfChild.transform.parent, Is.Not.Null);
            Assert.That(childOfChild.transform.parent, Is.SameAs(child.transform));
        }

        [Test]
        public void GameObjectsWillNotBeSetAsPrefabs()
        {
            Assert.That(root.isPrefab, Is.False);
            Assert.That(child.isPrefab, Is.False);
            Assert.That(childOfChild.isPrefab, Is.False);
            scene.AfterLoad();
            Assert.That(root.isPrefab, Is.False);
            Assert.That(child.isPrefab, Is.False);
            Assert.That(childOfChild.isPrefab, Is.False);
        }

        [Test]
        public void ComponentsWillNotBeSetAsPrefabs()
        {
            Assert.That(rootComponent.isPrefab, Is.False);
            Assert.That(childComponent.isPrefab, Is.False);
            Assert.That(childOfChildComponent.isPrefab, Is.False);
            scene.AfterLoad();
            Assert.That(rootComponent.isPrefab, Is.False);
            Assert.That(childComponent.isPrefab, Is.False);
            Assert.That(childOfChildComponent.isPrefab, Is.False);
        }

        [Test]
        public void AllPrefabGameObjectsWillBeSetAsPrefabs()
        {
            scene.prefabs.AddRange(scene.gameObjects);
            scene.gameObjects.Clear();
            Assert.That(root.isPrefab, Is.False);
            Assert.That(child.isPrefab, Is.False);
            Assert.That(childOfChild.isPrefab, Is.False);
            scene.AfterLoad();
            Assert.That(root.isPrefab, Is.True);
            Assert.That(child.isPrefab, Is.True);
            Assert.That(childOfChild.isPrefab, Is.True);
        }

        [Test]
        public void AllPrefabComponentsWillBeSetAsPrefabs()
        {
            scene.prefabs.AddRange(scene.gameObjects);
            scene.gameObjects.Clear();
            Assert.That(rootComponent.isPrefab, Is.False);
            Assert.That(childComponent.isPrefab, Is.False);
            Assert.That(childOfChildComponent.isPrefab, Is.False);
            scene.AfterLoad();
            Assert.That(rootComponent.isPrefab, Is.True);
            Assert.That(childComponent.isPrefab, Is.True);
            Assert.That(childOfChildComponent.isPrefab, Is.True);
        }

        [Test]
        public void WeCanFindGameObjectsOnTheScene()
        {
            Application.LoadLevel(scene);
            Assert.That(Application.Find(root.GetInstanceID()), Is.Not.Null);
            Assert.That(Application.Find(child.GetInstanceID()), Is.Not.Null);
            Assert.That(Application.Find(childOfChild.GetInstanceID()), Is.Not.Null);
        }

        [Test]
        public void WeCanFindComponentsOnTheScene()
        {
            Application.LoadLevel(scene);
            Assert.That(Application.Find(rootComponent.GetInstanceID()), Is.Not.Null);
            Assert.That(Application.Find(childComponent.GetInstanceID()), Is.Not.Null);
            Assert.That(Application.Find(childOfChildComponent.GetInstanceID()), Is.Not.Null);
        }

        [Test]
        public void WeCanFindPrefabObjectsOnTheScene()
        {
            scene.prefabs.AddRange(scene.gameObjects);
            scene.gameObjects.Clear();
            Application.LoadLevel(scene);
            Assert.That(Application.Find(root.GetInstanceID()), Is.Not.Null);
            Assert.That(Application.Find(child.GetInstanceID()), Is.Not.Null);
            Assert.That(Application.Find(childOfChild.GetInstanceID()), Is.Not.Null);
        }

        [Test]
        public void WeCanFindPrefabComponentsOnTheScene()
        {
            scene.prefabs.AddRange(scene.gameObjects);
            scene.gameObjects.Clear();
            Application.LoadLevel(scene);
            Assert.That(Application.Find(rootComponent.GetInstanceID()), Is.Not.Null);
            Assert.That(Application.Find(childComponent.GetInstanceID()), Is.Not.Null);
            Assert.That(Application.Find(childOfChildComponent.GetInstanceID()), Is.Not.Null);
        }

        [Test]
        public void AwakeWillBeCalledWhenTheSceneIsLoaded()
        {
            bool awakeCalled = false;
            rootComponent.onAwake = () => { awakeCalled = true; };

            Assert.That(awakeCalled, Is.False);
            Application.LoadLevel(scene);
            Assert.That(awakeCalled, Is.True);
        }

        [Test]
        public void AwakeWillNotBeCalledOnPrefabObjects()
        {
            bool awakeCalled = false;
            rootComponent.onAwake = () => { awakeCalled = true; };

            Assert.That(awakeCalled, Is.False);
            scene.prefabs.AddRange(scene.gameObjects);
            scene.gameObjects.Clear();
            Application.LoadLevel(scene);
            Assert.That(awakeCalled, Is.False);
        }


        [Test]
        public void AllGameObjectsWillExistWhenAwakeIsCalled()
        {
            bool componentsAreThere = false;
            rootComponent.onAwake = () =>
            {
                componentsAreThere = (Application.Find(root.GetInstanceID()) != null)
                                    && (Application.Find(child.GetInstanceID()) != null)
                                    && (Application.Find(childOfChild.GetInstanceID()) != null);
            };
            Assert.That(componentsAreThere, Is.False);
            Application.LoadLevel(scene);
            Assert.That(componentsAreThere, Is.True);
        }

        [Test]
        public void AllComponentsWillExistWhenAwakeIsCalled()
        {
            bool componentsAreThere = false;
            rootComponent.onAwake = () =>
            {
                componentsAreThere = (Application.Find(rootComponent.GetInstanceID()) != null)
                                    && (Application.Find(childComponent.GetInstanceID()) != null)
                                    && (Application.Find(childOfChildComponent.GetInstanceID()) != null);
            };
            Assert.That(componentsAreThere, Is.False);
            Application.LoadLevel(scene);
            Assert.That(componentsAreThere, Is.True);
        }

        [Test]
        public void IfAComponentIsCreatedDuringAwakeItWillBeAwoken()
        {
            bool awakeCalled = false;
            TestComponent newComponent = null;
            rootComponent.onAwake = () => { newComponent = new TestComponent() { onAwake = () => { awakeCalled = true; } }; };

            Assert.That(awakeCalled, Is.False);
            Application.LoadLevel(scene);
            Assert.That(newComponent, Is.Not.Null);
            Assert.That(awakeCalled, Is.True);
        }
	
    }
}
