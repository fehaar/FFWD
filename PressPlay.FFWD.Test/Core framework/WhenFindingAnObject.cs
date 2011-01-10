using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenFindingAnObject
    {
        class MyTestComponent : TestComponent
        {
        }

        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void WeWillNotFindComponentsWhenTheyHAventBeenAwoken()
        {
            GameObject go = new GameObject();
            go.AddComponent(typeof(TestComponent));

            UnityObject obj = Application.FindObjectOfType(typeof(TestComponent));

            Assert.That(obj, Is.Null);
        }

        [Test]
        public void WeWillFindAComponentByItsType()
        {
            GameObject go = new GameObject();
            go.AddComponent(typeof(TestComponent));
            Application.AwakeNewComponents();

            UnityObject obj = Application.FindObjectOfType(typeof(TestComponent));

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj, Is.TypeOf<TestComponent>());
        }

        [Test]
        public void WeWillFindTheCorrectSubType()
        {
            GameObject go = new GameObject();
            go.AddComponent(typeof(TestComponent));
            go.AddComponent(typeof(MyTestComponent));
            Application.AwakeNewComponents();

            UnityObject obj = Application.FindObjectOfType(typeof(MyTestComponent));

            Assert.That(obj, Is.Not.Null);
            Assert.That(obj, Is.TypeOf<MyTestComponent>());
        }
	

    }
}
