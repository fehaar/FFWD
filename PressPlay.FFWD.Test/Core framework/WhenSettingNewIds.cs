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
            obj.SetNewId();

            Assert.That(obj.GetInstanceID(), Is.Not.EqualTo(id));
        }

        [Test]
        public void WeWillSetTheIdOnTheComponentsOfAGameObject()
        {
            GameObject obj = new GameObject();
            TestComponent comp = new TestComponent();
            obj.AddComponent(comp);
            int id = comp.GetInstanceID();
            obj.SetNewId();

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
            obj.SetNewId();

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
            obj.SetNewId();

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
            parent.SetNewId();

            Assert.That(obj.GetInstanceID(), Is.Not.EqualTo(id));
            Assert.That(trans.GetInstanceID(), Is.Not.EqualTo(compId));
        }

        [Test]
        public void WeWillSetTheIdOfOurChildrensChildrenAndItsComponents()
        {
            TestHierarchy h = new TestHierarchy();

            int id = h.childOfChild.GetInstanceID();
            int compId = h.childOfChildTrans.GetInstanceID();
            h.root.SetNewId();

            Assert.That(h.childOfChild.GetInstanceID(), Is.Not.EqualTo(id));
            Assert.That(h.childOfChildTrans.GetInstanceID(), Is.Not.EqualTo(compId));
        }
	
	
    }
}
