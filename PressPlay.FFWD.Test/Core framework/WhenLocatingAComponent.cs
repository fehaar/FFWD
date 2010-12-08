using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenLocatingAComponent
    {
        GameObject root;
        GameObject child;
        GameObject childOfChild;

        [SetUp]
        public void Setup()
        {
            root = new GameObject();
            root.AddComponent(new Transform());
            child = new GameObject();
            child.AddComponent(new Transform());
            childOfChild = new GameObject();
            childOfChild.AddComponent(new Transform());
            childOfChild.transform.parent = child.transform;
            child.transform.parent = root.transform;
        }

        [Test]
        public void WeWillGetAnEmptyListWhenSearchingForComponentsInTheSameObject()
        {
            TestComponent[] components = root.GetComponents<TestComponent>();
            Assert.That(components, Is.Not.Null);
            Assert.That(components, Is.Empty);            
        }

        [Test]
        public void WeWillGetAnEmptyListWhenSearchingForComponentsInTheSameObjectNonGeneric()
        {
            Component[] components = root.GetComponents(typeof(TestComponent));
            Assert.That(components, Is.Not.Null);
            Assert.That(components, Is.Empty);
        }

        [Test]
        public void WeCanFindAComponentInTheSameObject()
        {
            TestComponent comp = new TestComponent();
            root.AddComponent(comp);
            TestComponent[] components = root.GetComponents<TestComponent>();
            Assert.That(components, Is.Not.Null);
            Assert.That(components, Contains.Item(comp));
        }

        [Test]
        public void WeCanFindAComponentInTheSameObjectNonGeneric()
        {
            TestComponent comp = new TestComponent();
            root.AddComponent(comp);
            Component[] components = root.GetComponents(typeof(TestComponent));
            Assert.That(components, Is.Not.Null);
            Assert.That(components, Contains.Item(comp));
        }	

        [Test]
        public void WeCanFindAComponentInTheParentObject()
        {
            TestComponent comp = new TestComponent();
            root.AddComponent(comp);
            TestComponent[] components = child.GetComponentsInParents<TestComponent>();
            Assert.That(components, Is.Not.Null);
            Assert.That(components, Contains.Item(comp));
        }

        [Test]
        public void WeCanFindAllComponentsInTheParentHierarchy()
        {
            TestComponent comp = new TestComponent();
            TestComponent comp1 = new TestComponent();
            TestComponent comp2 = new TestComponent();
            root.AddComponent(comp);
            child.AddComponent(comp1);
            child.AddComponent(comp2);
            TestComponent[] components = childOfChild.GetComponentsInParents<TestComponent>();
            Assert.That(components, Is.Not.Null);
            Assert.That(components, Contains.Item(comp));
            Assert.That(components, Contains.Item(comp1));
            Assert.That(components, Contains.Item(comp2));
        }

        [Test]
        public void WhenGettingObjectsInParentWeWillNotGetInOurOwnObject()
        {
            TestComponent comp = new TestComponent();
            child.AddComponent(comp);
            TestComponent[] components = child.GetComponentsInParents<TestComponent>();
            Assert.That(components, Is.Not.Null);
            Assert.That(components, Is.Empty);
        }

        [Test]
        public void WeCanGetAllComponentsOnTheCurrentScene()
        {
            Scene mainScene = new Scene();
            mainScene.gameObjects.Add(root);
            TestComponent comp = new TestComponent();
            TestComponent comp1 = new TestComponent();
            TestComponent comp2 = new TestComponent();
            root.AddComponent(comp);
            child.AddComponent(comp1);
            childOfChild.AddComponent(comp2);
            Application.LoadLevel(mainScene);

            UnityObject[] components = UnityObject.FindObjectsOfType(typeof(TestComponent));
            Assert.That(components, Is.Not.Null);
            Assert.That(components.Length, Is.EqualTo(3));
            Assert.That(components, Contains.Item(comp));
            Assert.That(components, Contains.Item(comp1));
            Assert.That(components, Contains.Item(comp2));
        }

        [Test]
        public void WeCanFindTheFirstComponentOfAGivenType()
        {
            Scene mainScene = new Scene();
            mainScene.gameObjects.Add(root);
            TestComponent comp = new TestComponent();
            child.AddComponent(comp);
            Application.LoadLevel(mainScene);

            UnityObject cmp = UnityObject.FindObjectOfType(typeof(TestComponent));
            Assert.That(cmp, Is.Not.Null);
        }


        //[Test]
        //public void WeWillFindTheDeepestComponentInTheHierarchy()
        //{
        //    Scene mainScene = new Scene();
        //    mainScene.gameObjects.Add(root);
        //    TestComponent comp = new TestComponent();
        //    TestComponent comp1 = new TestComponent();
        //    TestComponent comp2 = new TestComponent();
        //    root.AddComponent(comp);
        //    child.AddComponent(comp1);
        //    childOfChild.AddComponent(comp2);
        //    Application.LoadScene(mainScene);

        //    UnityObject cmp = UnityObject.FindObjectOfType(typeof(TestComponent));
        //    Assert.That(cmp, Is.Not.Null);
        //}
	
    }
}
