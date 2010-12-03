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
        GameObject go;
        GameObject child;
        GameObject childOfChild;
        TestComponent component;
        TestComponent childComponent;
        TestComponent childOfChildComponent;

        [SetUp]
        public void Setup()
        {
            scene = new Scene();
            go = new GameObject() { transform = new Transform() };
            component = new TestComponent();

            go.AddComponent(component);
            scene.gameObjects.Add(go);

            child = new GameObject() { transform = new Transform() };
            childComponent = new TestComponent();
            child.AddComponent(childComponent);
            go.transform.children = new List<GameObject>();
            go.transform.children.Add(child);

            childOfChild = new GameObject() { transform = new Transform() };
            childOfChildComponent = new TestComponent();
            childOfChild.AddComponent(childOfChildComponent);
            child.transform.children = new List<GameObject>();
            child.transform.children.Add(childOfChild);
        }

        [Test]
        public void AfterLoadWillEnsureThatAllReferencesAreSetOnComponents()
        {
            Assert.Inconclusive("This will only work when loading with the intermediate serializer, so we need to do a test like that.");
            Assert.That(component.gameObject, Is.Null);
            scene.AfterLoad();
            Assert.That(component.gameObject, Is.Not.Null);
            Assert.That(component.gameObject, Is.EqualTo(go));
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
            Assert.That(child.transform.parent, Is.SameAs(go.transform));
        }


        [Test]
        public void AfterLoadWillEnsureThatAllTransformParentReferencesAreSetAllTheWayThroughTheHierachy()
        {
            Assert.That(childOfChild.transform.parent, Is.Null);
            scene.AfterLoad();
            Assert.That(childOfChild.transform.parent, Is.Not.Null);
            Assert.That(childOfChild.transform.parent, Is.SameAs(child.transform));
        }

    }
}
