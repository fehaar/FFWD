using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenSettingNewIds
    {
        [Test]
        public void WeWillSetTheIdOnAGameObject()
        {
            GameObject obj = new GameObject();
            int id = obj.GetInstanceID();
            obj.SetNewId(new Dictionary<int, UnityObject>());

            Assert.That(obj.GetInstanceID(), Is.Not.EqualTo(id));
        }

        [Test]
        public void WeWillSetTheIdOnTheComponentsOfAGameObject()
        {
            GameObject obj = new GameObject();
            TestComponent comp = new TestComponent();
            obj.AddComponent(comp);
            int id = comp.GetInstanceID();
            obj.SetNewId(new Dictionary<int, UnityObject>());

            Assert.That(comp.GetInstanceID(), Is.Not.EqualTo(id));
        }

        [Test]
        public void WeWillSetTheIdOfAParentObject()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject parent = new GameObject();
            Transform parentTrans = parent.transform;
            trans.parent = parentTrans;
            int id = parent.GetInstanceID();
            obj.SetNewId(new Dictionary<int, UnityObject>());

            Assert.That(parent.GetInstanceID(), Is.Not.EqualTo(id));
        }

        [Test]
        public void WeWillSetTheIdOfComponentsOnAParentObject()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject parent = new GameObject();
            Transform parentTrans = parent.transform;
            trans.parent = parentTrans;
            int id = parentTrans.GetInstanceID();
            obj.SetNewId(new Dictionary<int, UnityObject>());

            Assert.That(parentTrans.GetInstanceID(), Is.Not.EqualTo(id));
        }

        [Test]
        public void WeWillSetTheIdOfOurChildrenAndItsComponents()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject parent = new GameObject();
            Transform parentTrans = parent.transform;
            trans.parent = parentTrans;
            int id = obj.GetInstanceID();
            int compId = trans.GetInstanceID();
            parent.SetNewId(new Dictionary<int, UnityObject>());

            Assert.That(obj.GetInstanceID(), Is.Not.EqualTo(id));
            Assert.That(trans.GetInstanceID(), Is.Not.EqualTo(compId));
        }

        [Test]
        public void WeWillSetTheIdOfOurChildrensChildrenAndItsComponents()
        {
            TestHierarchy h = new TestHierarchy();

            int id = h.childOfChild.GetInstanceID();
            int compId = h.childOfChildTrans.GetInstanceID();
            h.root.SetNewId(new Dictionary<int, UnityObject>());

            Assert.That(h.childOfChild.GetInstanceID(), Is.Not.EqualTo(id));
            Assert.That(h.childOfChildTrans.GetInstanceID(), Is.Not.EqualTo(compId));
        }

        [Test]
        public void TheIdMapWillContainTheObjectAndItsOriginalId()
        {
            Dictionary<int, UnityObject> ids = new Dictionary<int, UnityObject>();
            GameObject go = new GameObject();

            go.SetNewId(ids);

            Assert.That(ids.Count, Is.EqualTo(2));
            Assert.That(ids.ContainsValue(go), Is.True);
            Assert.That(ids.ContainsValue(go.transform), Is.True);
        }

        [Test]
        public void TheIdMapWillContainAMapOfAllComponents()
        {
            Dictionary<int, UnityObject> ids = new Dictionary<int, UnityObject>();
            GameObject go = new GameObject();
            go.AddComponent(typeof(TestComponent));
            go.AddComponent(typeof(TestComponent));

            go.SetNewId(ids);

            Assert.That(ids.Count, Is.EqualTo(4));
            foreach (var item in go.GetComponents(typeof(Component)))
            {
                Assert.That(ids.ContainsValue(item), Is.True);
            }
        }

        [Test]
        public void TheIdMapWillContainAMapOfAllChildren()
        {
            TestHierarchy h = new TestHierarchy();
            Dictionary<int, UnityObject> ids = new Dictionary<int, UnityObject>();

            h.root.SetNewId(ids);
            Assert.That(ids.ContainsValue(h.root), Is.True);
            Assert.That(ids.ContainsValue(h.child), Is.True);
            Assert.That(ids.ContainsValue(h.childOfChild), Is.True);
        }

        [Test]
        public void PrefabsWillNotGetNewIds()
        {
            GameObject go = new GameObject(true);
            TestComponent cmp = new TestComponent();
            go.AddComponent(cmp);

            int oldId = go.GetInstanceID();
            int oldCmpId = cmp.GetInstanceID();
            go.SetNewId(new Dictionary<int, UnityObject>());

            Assert.That(go.GetInstanceID(), Is.EqualTo(oldId));
            Assert.That(cmp.GetInstanceID(), Is.EqualTo(oldCmpId));
        }

        [Test]
        public void TheIdMapWillNotContainPrefabs()
        {
            GameObject go = new GameObject(true);
            TestComponent cmp = new TestComponent();
            go.AddComponent(cmp);

            Dictionary<int, UnityObject> ids = new Dictionary<int, UnityObject>();
            go.SetNewId(ids);

            Assert.That(ids.Count, Is.EqualTo(0));
        }
    }
}
