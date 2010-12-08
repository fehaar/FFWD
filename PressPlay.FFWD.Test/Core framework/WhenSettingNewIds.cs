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
            Transform trans = new Transform();
            obj.AddComponent(trans);
            GameObject parent = new GameObject();
            Transform parentTrans = new Transform();
            parent.AddComponent(parentTrans);
            trans.parent = parentTrans;
            int id = parent.GetInstanceID();
            obj.SetNewId();

            Assert.That(parent.GetInstanceID(), Is.Not.EqualTo(id));
        }

        [Test]
        public void WeWillSetTheIdOfComponentsOnAParentObject()
        {
            GameObject obj = new GameObject();
            Transform trans = new Transform();
            obj.AddComponent(trans);
            GameObject parent = new GameObject();
            Transform parentTrans = new Transform();
            parent.AddComponent(parentTrans);
            trans.parent = parentTrans;
            int id = parentTrans.GetInstanceID();
            obj.SetNewId();

            Assert.That(parentTrans.GetInstanceID(), Is.Not.EqualTo(id));
        }

        [Test]
        public void WeWillSetTheIdOfOurChildrenAndItsComponents()
        {
            GameObject obj = new GameObject();
            Transform trans = new Transform();
            obj.AddComponent(trans);
            GameObject parent = new GameObject();
            Transform parentTrans = new Transform();
            parent.AddComponent(parentTrans);
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
            GameObject root = new GameObject();
            Transform rootTrans = new Transform();
            root.AddComponent(rootTrans);

            GameObject child = new GameObject();
            Transform childTrans = new Transform();
            child.AddComponent(childTrans);
            childTrans.parent = rootTrans;

            GameObject childOfchild = new GameObject();
            Transform childOfchildTrans = new Transform();
            childOfchild.AddComponent(childOfchildTrans);
            childOfchildTrans.parent = childTrans;

            int id = childOfchild.GetInstanceID();
            int compId = childOfchildTrans.GetInstanceID();
            root.SetNewId();

            Assert.That(childOfchild.GetInstanceID(), Is.Not.EqualTo(id));
            Assert.That(childOfchildTrans.GetInstanceID(), Is.Not.EqualTo(compId));
        }
	
	
    }
}
