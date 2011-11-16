using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Instantiate
{
    [TestFixture]
    public class WhenAnInstantiatedObjectHaveReferences
    {
        [Test]
        public void ReferencesBackToComponentsInTheClonedObjectWillBeChanged()
        {
            GameObject obj = new GameObject();
            ReferencingComponent cmp = new ReferencingComponent();
            obj.AddComponent(cmp);
            cmp.reference = obj.transform;

            GameObject clone = (GameObject)GameObject.Instantiate(obj);
            ReferencingComponent cloneCmp = clone.GetComponent<ReferencingComponent>();
            Assert.That(cloneCmp.reference, Is.Not.SameAs(obj.transform));
            Assert.That(cloneCmp.reference, Is.SameAs(clone.transform));
        }

        [Test]
        public void ArraysOfReferencesWillCreateANewArrayAndReplaceReference()
        {
            GameObject obj = new GameObject();
            ReferencingComponent cmp = new ReferencingComponent();
            obj.AddComponent(cmp);
            cmp.componentArray = new Component[] { obj.AddComponent<TestComponent>(), obj.AddComponent<TestComponent>(), obj.AddComponent<TestComponent>() };

            GameObject clone = (GameObject)GameObject.Instantiate(obj);
            ReferencingComponent clonedCmp = clone.GetComponent<ReferencingComponent>();
            Assert.That(clonedCmp.componentArray, Is.Not.SameAs(cmp.componentArray));
            for (int i = 0; i < clonedCmp.componentArray.Length; i++)
            {
                Assert.That(clonedCmp.componentArray[i], Is.Not.Null);
                Assert.That(clonedCmp.componentArray[i], Is.Not.SameAs(cmp.componentArray[i]));
            }
        }
    }
}
