using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.U2X.Xna.Test.Core_framework.Transforms
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
	
	
    }
}
