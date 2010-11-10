using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace PressPlay.U2X.Xna.Test.Core_framework.Transforms
{
    [TestFixture]
    public class WhenManipulatingTransforms
    {
        [Test]
        public void AChildWillBePositionedRelativeToItsParent()
        {
            Transform trans = new Transform() { localPosition = Vector3.One };
            Transform child = new Transform() { localPosition = Vector3.Up };

            Assert.That(child.position, Is.EqualTo(Vector3.Up));
            child.parent = trans;
            Assert.That(child.position, Is.EqualTo(Vector3.One + Vector3.Up));
        }

        [Test]
        public void MovingAParentWillMoveItsChild()
        {
            Transform trans = new Transform() { localPosition = Vector3.One };
            Transform child = new Transform() { localPosition = Vector3.Up };
            child.parent = trans;
            Assert.That(child.position, Is.EqualTo(Vector3.One + Vector3.Up));
            trans.localPosition = new Vector3(2, 2, 2);
            Assert.That(child.position, Is.EqualTo(new Vector3(2, 2, 2) + Vector3.Up));
        }
	
	
    }
}
