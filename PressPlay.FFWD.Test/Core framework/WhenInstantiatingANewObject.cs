using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenInstantiatingANewObject
    {
        [Test]
        public void WeWillGetACopyWithANewId()
        {
            GameObject obj = new GameObject() { name = "MyObject", active = true, layer = 10, tag = "Mytag" };
            GameObject instance = GameObject.Instantiate(obj) as GameObject;
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.GetInstanceID(), Is.Not.EqualTo(obj.GetInstanceID()));
            Assert.That(instance.name, Is.EqualTo(obj.name + "(Clone)"));
            Assert.That(instance.active, Is.True);
            Assert.That(instance.layer, Is.EqualTo(obj.layer));
            Assert.That(instance.tag, Is.EqualTo(obj.tag));
        }

        [Test]
        public void WeWillGetTheTransformCloned()
        {
            GameObject obj = new GameObject();
            Transform trans = new Transform();
            trans.localPosition = Vector3.UnitX;
            obj.AddComponent(trans);
            GameObject instance = GameObject.Instantiate(obj) as GameObject;
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.transform, Is.Not.Null);
            Assert.That(instance.transform.GetInstanceID(), Is.Not.EqualTo(trans.GetInstanceID()));
            Assert.That(instance.transform.localPosition, Is.EqualTo(trans.localPosition));
            Assert.That(instance.transform, Is.Not.SameAs(trans));
        }

        [Test]
        public void WeWillGetAllComponentsCloned()
        {
            GameObject obj = new GameObject();
            obj.AddComponent(new TestComponent());
            obj.AddComponent(new TestComponent());
            obj.AddComponent(new TestComponent());
            obj.AddComponent(new TestComponent());
            GameObject instance = GameObject.Instantiate(obj) as GameObject;
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.GetComponents<TestComponent>(), Has.Length.EqualTo(4));
        }

        [Test]
        public void WeCanPlaceTheNewComponent()
        {
            GameObject obj = new GameObject();
            Transform trans = new Transform();
            obj.AddComponent(trans);
            Vector3 pos = Vector3.UnitZ;
            Quaternion rot = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 10);
            GameObject instance = GameObject.Instantiate(obj, pos, rot) as GameObject;
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.transform.localPosition, Is.EqualTo(pos));
            Assert.That(instance.transform.localRotation, Is.EqualTo(rot));
        }

        [Test]
        public void WeCanInstantiateAComponent()
        {
            GameObject obj = new GameObject();
            TestComponent comp = new TestComponent();
            obj.AddComponent(comp);

            TestComponent comp1 = Component.Instantiate(comp) as TestComponent;
            Assert.That(comp1, Is.Not.Null);
            Assert.That(comp1, Is.Not.SameAs(comp));
            Assert.That(comp1.GetInstanceID(), Is.Not.EqualTo(comp.GetInstanceID()));
        }

        [Test]
        public void AnInstantiatedComponentWillHaveACloneOfTheGameObjectAsWell()
        {
            GameObject obj = new GameObject() { name = "MyObject", active = true, layer = 10, tag = "Mytag" };
            TestComponent comp = new TestComponent();
            obj.AddComponent(comp);

            TestComponent comp1 = Component.Instantiate(comp) as TestComponent;
            Assert.That(comp1, Is.Not.Null);
            Assert.That(comp1.gameObject, Is.Not.Null);
            Assert.That(comp1.gameObject, Is.Not.EqualTo(obj));
            Assert.That(comp1.gameObject.name, Is.EqualTo(obj.name + "(Clone)"));
            Assert.That(comp1.gameObject.active, Is.True);
            Assert.That(comp1.gameObject.layer, Is.EqualTo(obj.layer));
            Assert.That(comp1.gameObject.tag, Is.EqualTo(obj.tag));
        }

        [Test]
        public void WeWillCloneChildObjects()
        {
            GameObject obj = new GameObject();
            Transform trans = new Transform();
            obj.AddComponent(trans);
            GameObject child = new GameObject() { name = "MyObject", active = true, layer = 10, tag = "Mytag" };
            Transform trans1 = new Transform();
            child.AddComponent(trans1);
            trans1.parent = trans;

            GameObject clone = GameObject.Instantiate(obj) as GameObject;
            Assert.That(clone, Is.Not.Null);
            Assert.That(clone.transform.children, Has.Count.EqualTo(1));
            GameObject cloneChild = clone.transform.children[0];
            Assert.That(cloneChild, Is.Not.SameAs(child));
            Assert.That(cloneChild.GetInstanceID(), Is.Not.EqualTo(child.GetInstanceID()));
            Assert.That(cloneChild.name, Is.EqualTo(child.name + "(Clone)"));
            Assert.That(cloneChild.active, Is.True);
            Assert.That(cloneChild.layer, Is.EqualTo(child.layer));
            Assert.That(cloneChild.tag, Is.EqualTo(child.tag));
        }

        [Test]
        public void WeWillCloneComponentsOnChildObjects()
        {
            GameObject obj = new GameObject();
            Transform trans = new Transform();
            obj.AddComponent(trans);
            GameObject child = new GameObject() { name = "MyObject", active = true, layer = 10, tag = "Mytag" };
            Transform trans1 = new Transform();
            child.AddComponent(trans1);
            trans1.parent = trans;
            TestComponent comp = new TestComponent();
            child.AddComponent(comp);

            GameObject clone = GameObject.Instantiate(obj) as GameObject;
            GameObject cloneChild = clone.transform.children[0];
            TestComponent childComp = cloneChild.GetComponent<TestComponent>();
            Assert.That(childComp, Is.Not.Null);
            Assert.That(childComp, Is.Not.SameAs(comp));
        }

        [Test]
        public void WeWillCloneAParentObject()
        {
            GameObject obj = new GameObject() { name = "MyObject", active = true, layer = 10, tag = "Mytag" };
            Transform trans = new Transform();
            obj.AddComponent(trans);
            GameObject child = new GameObject();
            Transform trans1 = new Transform();
            child.AddComponent(trans1);
            trans1.parent = trans;

            GameObject clone = GameObject.Instantiate(child) as GameObject;
            Assert.That(clone.transform, Is.Not.Null);
            Assert.That(clone.transform.parent, Is.Not.Null);
            Assert.That(clone.transform.parent, Is.Not.SameAs(trans));
            GameObject cloneParent = clone.transform.parent.gameObject;
            Assert.That(cloneParent, Is.Not.SameAs(obj));
            Assert.That(cloneParent.name, Is.EqualTo(obj.name + "(Clone)"));
            Assert.That(cloneParent.GetInstanceID(), Is.Not.EqualTo(obj.GetInstanceID()));
            Assert.That(cloneParent.active, Is.True);
            Assert.That(cloneParent.layer, Is.EqualTo(obj.layer));
            Assert.That(cloneParent.tag, Is.EqualTo(obj.tag));
        }

        [Test]
        public void WeWillCloneAParentHierarchy()
        {
            GameObject root = new GameObject();
            Transform rootTrans = new Transform();
            root.AddComponent(rootTrans);

            GameObject child = new GameObject();
            Transform childTrans = new Transform();
            child.AddComponent(childTrans);
            childTrans.parent = rootTrans;

            GameObject childOfChild = new GameObject();
            Transform childOfChildTrans = new Transform();
            childOfChild.AddComponent(childOfChildTrans);
            childOfChildTrans.parent = childTrans;

            GameObject clone = GameObject.Instantiate(childOfChild) as GameObject;
            Assert.That(clone.transform.parent, Is.Not.Null);
            Assert.That(clone.transform.parent.transform.parent, Is.Not.Null);
            Assert.That(clone.transform.parent.transform.parent.transform.parent, Is.Null);
        }

        [Test]
        public void WeWillCloneAParentHierarchyByComponent()
        {
            GameObject root = new GameObject();
            Transform rootTrans = new Transform();
            root.AddComponent(rootTrans);

            GameObject child = new GameObject();
            Transform childTrans = new Transform();
            child.AddComponent(childTrans);
            childTrans.parent = rootTrans;

            GameObject childOfChild = new GameObject();
            Transform childOfChildTrans = new Transform();
            childOfChild.AddComponent(childOfChildTrans);
            childOfChildTrans.parent = childTrans;
            TestComponent comp = new TestComponent();
            childOfChild.AddComponent(comp);

            TestComponent clone = GameObject.Instantiate(comp) as TestComponent;
            Assert.That(clone, Is.Not.Null);
            Assert.That(clone.transform.parent, Is.Not.Null);
            Assert.That(clone.transform.parent.transform.parent, Is.Not.Null);
            Assert.That(clone.transform.parent.transform.parent.transform.parent, Is.Null);
        }

        [Test]
        public void NewlyInstantiatedObjectsWillBeActive()
        {
            GameObject obj = new GameObject() { name = "MyObject", active = false, layer = 10, tag = "Mytag" };
            GameObject instance = GameObject.Instantiate(obj) as GameObject;
            Assert.That(instance.active, Is.True);
        }
    }
}
