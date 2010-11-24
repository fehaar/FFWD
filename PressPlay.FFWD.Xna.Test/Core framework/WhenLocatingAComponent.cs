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
            root = new GameObject() { transform = new Transform() };
            child = new GameObject() { transform = new Transform() };
            childOfChild = new GameObject() { transform = new Transform() };
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
        public void WeCanFindAComponentInTheSameObject()
        {
            TestComponent comp = new TestComponent();
            root.components.Add(comp);
            TestComponent[] components = root.GetComponents<TestComponent>();
            Assert.That(components, Is.Not.Null);
            Assert.That(components, Contains.Item(comp));
        }	

        [Test]
        public void WeCanFindAComponentInTheParentObject()
        {
            TestComponent comp = new TestComponent();
            root.components.Add(comp);
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
            root.components.Add(comp);
            child.components.Add(comp1);
            child.components.Add(comp2);
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
            child.components.Add(comp);
            TestComponent[] components = child.GetComponentsInParents<TestComponent>();
            Assert.That(components, Is.Not.Null);
            Assert.That(components, Is.Empty);
        }
	
    }
}
