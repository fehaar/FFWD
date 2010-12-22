using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Transforms
{
    [TestFixture]
    public class WhenManipulatingTheTransformHierarchy
    {
        [Test]
        public void AChildCanBeAddedBySettingItsParent()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;

            child.parent = trans;

            Assert.That(trans.children, Is.Not.Null);
            Assert.That(trans.children, Contains.Item(child.gameObject));
        }

        [Test]
        public void AChildCanBeRemovedBySettingTheParentToNull()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;

            child.parent = trans;
            child.parent = null;
            Assert.That(trans.children, Has.No.Member(child.gameObject));            
        }

        [Test]
        public void AChildCanBeMovedToAnotherParent()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;
            GameObject newObj = new GameObject();

            child.parent = trans;
            child.parent = newObj.transform;
            Assert.That(trans.children, Has.No.Member(child.gameObject));
            Assert.That(newObj.transform.children, Has.Member(child.gameObject));
        }

        [Test]
        public void ATransformWithNoParentHasItselfAsRoot()
        {
            Transform trans = new Transform();
            Assert.That(trans.root, Is.SameAs(trans));
        }

        [Test]
        public void ATransformWithAParentWillHaveItAsRoot()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;

            child.parent = trans;
            Assert.That(child.root, Is.SameAs(trans));
        }

        [Test]
        public void InADeepHiearachyRootWillBeTheTopmostElement()
        {
            TestHierarchy h = new TestHierarchy();
            Assert.That(h.childOfChildTrans.root, Is.SameAs(h.rootTrans));
        }

        [Test]
        public void WhenSettingTheParentOfATransformTheGlobalPositionWillNotChange()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;

            trans.localPosition = new Vector3(2, 3, 4);
            child.localPosition = new Vector3(2, 3, 4);

            Vector3 childPos = child.position;
            child.parent = trans;
            Assert.That(child.position, Is.EqualTo(childPos));
        }

        [Test]
        public void WhenSettingTheParentOfATransformTheGlobalPositionWillNotChangeEvenWhenRotated()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;

            trans.localPosition = new Vector3(2, 3, 4);
            trans.localRotation = Quaternion.Euler(MathHelper.PiOver2, 0.0f, 0.0f);
            child.localPosition = new Vector3(2, 2, 2);

            Vector3 childPos = child.position;
            child.parent = trans;
            Assert.That((child.position - childPos).sqrMagnitude, Is.LessThan(0.000001f));
        }

        [Test]
        public void WhenSettingTheParentOfATransformTheLossyScaleWillNotChange()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");

        }
	
    }
}
