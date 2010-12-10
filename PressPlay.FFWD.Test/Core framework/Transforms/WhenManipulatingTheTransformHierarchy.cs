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
            Transform trans = new Transform();
            Transform child = new Transform();
            child.parent = trans;
            Assert.That(trans.children, Is.Not.Null);
            Assert.That(trans.children, Contains.Item(child.gameObject));
        }

        [Test]
        public void AChildCanBeRemovedBySettingTheParentToNull()
        {
            Transform trans = new Transform();
            Transform child = new Transform();
            child.parent = trans;
            child.parent = null;
            Assert.That(trans.children, Has.No.Member(child.gameObject));            
        }

        [Test]
        public void AChildCanBeMovedToAnotherParent()
        {
            Transform trans = new Transform();
            Transform newParent = new Transform();
            Transform child = new Transform();
            child.parent = trans;
            child.parent = newParent;
            Assert.That(trans.children, Has.No.Member(child.gameObject));
            Assert.That(newParent.children, Has.Member(child.gameObject));
        }

        [Test]
        public void WhenSettingTheParentOfATransformTheGlobalPositionWillNotChange()
        {
            Transform trans = new Transform() { localPosition = new Vector3(2, 3, 4) };
            Transform child = new Transform() { localPosition = new Vector3(2, 3, 4) };

            Vector3 childPos = child.position;
            child.parent = trans;
            Assert.That(child.position, Is.EqualTo(childPos));
        }

        [Test]
        public void WhenSettingTheParentOfATransformTheGlobalPositionWillNotChangeEvenWhenRotated()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");

        }

        [Test]
        public void WhenSettingTheParentOfATransformTheLossyScaleWillNotChange()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");

        }
	
    }
}
