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
        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

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
            Transform trans = obj.transform;
            trans.localPosition = Vector3.right;
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
        public void WeCanInstantiateAComponent()
        {
            GameObject obj = new GameObject();
            TestComponent comp = new TestComponent();
            obj.AddComponent(comp);

            TestComponent comp1 = Component.Instantiate(comp) as TestComponent;
            Assert.That(comp1, Is.Not.Null);
            Assert.That(comp1, Is.Not.SameAs(comp));
            Assert.That(comp1.GetInstanceID(), Is.Not.EqualTo(comp.GetInstanceID()));
            Assert.That(comp1.gameObject, Is.Not.SameAs(obj));
            Assert.That(comp1.gameObject.GetInstanceID(), Is.Not.EqualTo(comp.gameObject.GetInstanceID()));
        }

        [Test]
        public void WeWillInstantiateTheCorrectComponent()
        {
            GameObject obj = new GameObject();
            TestComponent comp = new TestComponent();
            TestComponent comp1 = new TestComponent() { Tag = "This" };
            obj.AddComponent(comp);
            obj.AddComponent(comp1);

            TestComponent inst = Component.Instantiate(comp1) as TestComponent;
            Assert.That(inst.Tag, Is.EqualTo(comp1.Tag));
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
            Transform trans = obj.transform;
            GameObject child = new GameObject() { name = "MyObject", active = true, layer = 10, tag = "Mytag" };
            Transform trans1 = child.transform;
            trans1.parent = trans;

            GameObject clone = GameObject.Instantiate(obj) as GameObject;
            Assert.That(clone, Is.Not.Null);
            Assert.That(clone.transform.childCount, Is.EqualTo(1));

            foreach (GameObject cloneChild in clone.transform)
            {
                Assert.That(cloneChild, Is.Not.SameAs(child));
                Assert.That(cloneChild.GetInstanceID(), Is.Not.EqualTo(child.GetInstanceID()));
                Assert.That(cloneChild.name, Is.EqualTo(child.name + "(Clone)"));
                Assert.That(cloneChild.active, Is.True);
                Assert.That(cloneChild.layer, Is.EqualTo(child.layer));
                Assert.That(cloneChild.tag, Is.EqualTo(child.tag));    
            }
        }

        [Test]
        public void WeWillCloneComponentsOnChildObjects()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject child = new GameObject() { name = "MyObject", active = true, layer = 10, tag = "Mytag" };
            Transform trans1 = child.transform;
            trans1.parent = trans;
            TestComponent comp = new TestComponent();
            child.AddComponent(comp);

            GameObject clone = GameObject.Instantiate(obj) as GameObject;

            foreach (GameObject cloneChild in clone.transform)
            {
                TestComponent childComp = cloneChild.GetComponent<TestComponent>();
                Assert.That(childComp, Is.Not.Null);
                Assert.That(childComp, Is.Not.SameAs(comp));
                Assert.That(childComp.gameObject, Is.SameAs(cloneChild));
            }
        }

        [Test]
        public void WeWillCloneAParentObject()
        {
            GameObject obj = new GameObject() { name = "MyObject", active = true, layer = 10, tag = "Mytag" };
            Transform trans = obj.transform;
            GameObject child = new GameObject();
            Transform trans1 = child.transform;
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
            TestHierarchy h = new TestHierarchy();

            GameObject clone = GameObject.Instantiate(h.childOfChild) as GameObject;
            Assert.That(clone.transform.parent, Is.Not.Null);
            Assert.That(clone.transform.parent.transform.parent, Is.Not.Null);
            Assert.That(clone.transform.parent.transform.parent.transform.parent, Is.Null);
        }

        [Test]
        public void WeWillCloneAParentHierarchyByComponent()
        {
            TestHierarchy h = new TestHierarchy();

            TestComponent comp = new TestComponent();
            h.childOfChild.AddComponent(comp);

            TestComponent clone = GameObject.Instantiate(comp) as TestComponent;
            Assert.That(clone, Is.Not.Null);
            Assert.That(clone.transform.parent, Is.Not.Null);
            Assert.That(clone.transform.parent.transform.parent, Is.Not.Null);
            Assert.That(clone.transform.parent.transform.parent.transform.parent, Is.Null);
        }

        [Test]
        public void IfWeHaveReferencedTheTransformOnTheGameObjectItWillBeCleared()
        {
            GameObject root = new GameObject();
            Transform rootTrans = root.transform;
            Transform callTrans = root.transform;

            GameObject clone = (GameObject)GameObject.Instantiate(root);
            Assert.That(clone.transform, Is.Not.SameAs(callTrans));
        }

        [Test]
        public void NewlyInstantiatedObjectsWillBeActive()
        {
            GameObject obj = new GameObject() { name = "MyObject", active = false, layer = 10, tag = "Mytag" };
            GameObject instance = GameObject.Instantiate(obj) as GameObject;
            Assert.That(instance.active, Is.True);
        }

        [Test]
        public void InstantiatingAPrefabWillMakeNonPrefabObjects()
        {
            GameObject obj = new GameObject() { isPrefab = true };
            TestComponent comp = new TestComponent();
            obj.AddComponent(comp);

            TestComponent instance = GameObject.Instantiate(comp) as TestComponent;
            Assert.That(instance.isPrefab, Is.False);
            Assert.That(instance.gameObject.isPrefab, Is.False);
        }

        [Test]
        public void InstantiatingAComponentInAwakeWillAwakeThemImmidiately()
        {
            int awakeCalls = 0;
            GameObject objPrefab = new GameObject(true);
            TestComponent compPrefab = new TestComponent() { onAwake = () => { awakeCalls++; } };
            objPrefab.AddComponent(compPrefab);
            Assert.That(awakeCalls, Is.EqualTo(1));

            TestComponent inst = (TestComponent)UnityObject.Instantiate(compPrefab);
            Assert.That(awakeCalls, Is.EqualTo(2));
        }

        [Test]
        public void WeCanPlaceTheNewGameObject()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            Vector3 pos = Vector3.forward;
            Quaternion rot = Quaternion.AngleAxis(10, Vector3.up);
            GameObject instance = GameObject.Instantiate(obj, pos, rot) as GameObject;
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.transform.localPosition, Is.EqualTo(pos));
            Assert.That(instance.transform.localRotation, Is.EqualTo(rot));
        }


        [Test]
        public void WeCanPlaceTheNewGameObjectWhenCloningByComponent()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            Vector3 pos = Vector3.forward;
            Quaternion rot = Quaternion.AngleAxis(10, Vector3.up);
            Transform instance = GameObject.Instantiate(trans, pos, rot) as Transform;
            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.gameObject.transform.localPosition, Is.EqualTo(pos));
            Assert.That(instance.gameObject.transform.localRotation, Is.EqualTo(rot));
        }

        [Test]
        public void AfterInstantiatingANewObjectItExists()
        {
            Assert.Inconclusive("I am not sure if this is the correct behaviour...");
            GameObject obj = new GameObject();
            GameObject clone = (GameObject)GameObject.Instantiate(obj);
            Assert.That(Application.Find(clone.GetInstanceID()), Is.Not.Null);
        }

    }
}
