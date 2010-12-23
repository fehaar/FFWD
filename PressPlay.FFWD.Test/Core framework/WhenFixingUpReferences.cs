using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenFixingUpReferences
    {
        [Test]
        public void WeWillReplaceADummyPrefabReferenceWithAReferenceFromTheIdMap()
        {
            GameObject go = new GameObject();
            TestComponent testCmp = new TestComponent();
            TestComponent newCmp = new TestComponent();
            ReferencingComponent cmp = new ReferencingComponent() { reference = testCmp };
            go.AddComponent(cmp);
            Dictionary<int, UnityObject> dict = new Dictionary<int, UnityObject>();
            dict.Add(testCmp.GetInstanceID(), newCmp);

            cmp.FixReferences(dict);

            Assert.That(cmp.reference, Is.SameAs(newCmp));
        }

        [Test]
        public void WeFixUpTheComponentsOfAGameObject()
        {
            GameObject go = new GameObject();
            TestComponent testCmp = new TestComponent();
            TestComponent newCmp = new TestComponent();
            ReferencingComponent cmp = new ReferencingComponent() { reference = testCmp };
            go.AddComponent(cmp);
            Dictionary<int, UnityObject> dict = new Dictionary<int, UnityObject>();
            dict.Add(testCmp.GetInstanceID(), newCmp);

            go.FixReferences(dict);

            Assert.That(cmp.reference, Is.SameAs(newCmp));
        }

        [Test]
        public void WeWillFixReferencesInTheHierarchy()
        {
            TestHierarchy h = new TestHierarchy();
            TestComponent testCmp = new TestComponent();
            TestComponent newCmp = new TestComponent();
            ReferencingComponent cmp = new ReferencingComponent() { reference = testCmp };
            h.childOfChild.AddComponent(cmp);
            Dictionary<int, UnityObject> dict = new Dictionary<int, UnityObject>();
            dict.Add(testCmp.GetInstanceID(), newCmp);

            h.root.FixReferences(dict);

            Assert.That(cmp.reference, Is.SameAs(newCmp));
        }
    }
}
