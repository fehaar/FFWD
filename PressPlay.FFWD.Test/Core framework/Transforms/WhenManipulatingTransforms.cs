using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Transforms
{
    [TestFixture]
    public class WhenManipulatingTransforms
    {
        [Test]
        public void AChildWillBePositionedRelativeToItsParent()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;

            trans.localPosition = new Vector3(2, 2, 2);
            child.localPosition = new Vector3(3, 2, 1);

            Assert.That(trans.position, Is.EqualTo(new Vector3(2, 2, 2)));
            Assert.That(child.position, Is.EqualTo(new Vector3(3, 2, 1)));
            child.parent = trans;
            Assert.That(child.position, Is.EqualTo(new Vector3(3, 2, 1)));
        }

        [Test]
        public void MovingAParentWillMoveItsChild()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;

            trans.localPosition = Vector3.one;
            child.localPosition = Vector3.up;

            child.parent = trans;
            Assert.That(child.position, Is.EqualTo(Vector3.up));            
            trans.localPosition = new Vector3(2, 2, 2);
            Assert.That(child.position, Is.EqualTo(new Vector3(2, 2, 2) + child.localPosition));
        }

        [Test]
        public void MovingAParentWillMoveAllChildren()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;
            GameObject child2Obj = new GameObject();
            Transform child2 = child2Obj.transform;

            trans.localPosition = Vector3.one;
            child.localPosition = Vector3.up;
            child2.localPosition = Vector3.up;

            child.parent = trans;
            child2.parent = trans;
            Assert.That(child.position, Is.EqualTo(Vector3.up));
            Assert.That(child2.position, Is.EqualTo(Vector3.up));
            trans.localPosition = new Vector3(2, 2, 2);
            Assert.That(child.position, Is.EqualTo(new Vector3(2, 2, 2) + child.localPosition));
            Assert.That(child2.position, Is.EqualTo(new Vector3(2, 2, 2) + child2.localPosition));
        }

        [Test]
        public void AChildWillBeScaledRelativielyToItsParent()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;

            trans.localScale = new Vector3(2, 2, 2);
            child.localScale = new Vector3(3, 2, 1);

            Assert.That(trans.lossyScale, Is.EqualTo(new Vector3(2, 2, 2)));
            Assert.That(child.lossyScale, Is.EqualTo(new Vector3(3, 2, 1)));
            child.parent = trans;
            Assert.That(child.lossyScale, Is.EqualTo(new Vector3(2, 2, 2) * new Vector3(3, 2, 1)));
        }

        [Test]
        public void ScalingAParentWillScaleTheChild()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;
            trans.localScale = Vector3.one;
            child.localScale = new Vector3(3, 2, 1);

            child.parent = trans;
            Assert.That(child.lossyScale, Is.EqualTo(new Vector3(3, 2, 1)));
            trans.localScale = new Vector3(2, 2, 2);
            Assert.That(child.lossyScale, Is.EqualTo(new Vector3(2, 2, 2) * new Vector3(3, 2, 1)));
        }

        [Test]
        public void RotatingAParentWillRotateTheChild()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;
            trans.localRotation = Quaternion.identity;
            child.localRotation = Quaternion.identity;

            Assert.That(trans.rotation, Is.EqualTo(Quaternion.identity));
            Assert.That(child.rotation, Is.EqualTo(Quaternion.identity));
            child.parent = trans;
            trans.localRotation = Quaternion.Euler(MathHelper.PiOver2, 0.0f, 0.0f);
            Assert.That(child.rotation, Is.Not.EqualTo(Quaternion.identity));
        }

        [Test]
        public void RotatingAParentWillMoveATranslatedChild()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;
            trans.localPosition = new Vector3(2, 2, 2);
            child.localPosition = new Vector3(3, 2, 1);

            Assert.That(trans.rotation, Is.EqualTo(Quaternion.identity));
            Assert.That(child.rotation, Is.EqualTo(Quaternion.identity));
            Vector3 beforeRotate = child.position;
            child.parent = trans;
            trans.localRotation = Quaternion.Euler(MathHelper.PiOver2, 0.0f, 0.0f);
            Assert.That(child.position, Is.Not.EqualTo(beforeRotate));
        }

        [Test]
        public void TheLocalPositionWillMoveWithThePositionIfWeHaveNoParent()
        {
            Transform trans = new Transform() { localPosition = new Vector3(2, 2, 2) };
            trans.position = new Vector3(3, 4, 5);
            Assert.That(trans.localPosition, Is.EqualTo(trans.position));
            Assert.That(trans.position, Is.EqualTo(new Vector3(3, 4, 5)));
        }

        [Test]
        public void TheLocalPositionWillChangeIfWeSetThePositionAndHaveAParent()
        {
            GameObject obj = new GameObject();
            Transform trans = obj.transform;
            GameObject childObj = new GameObject();
            Transform child = childObj.transform;

            trans.localPosition = new Vector3(2, 2, 2);
            Vector3 childLocal = new Vector3(3, 2, 1);
            child.localPosition = childLocal;

            child.parent = trans;

            Vector3 newPos = childLocal + new Vector3(10, 10, 10);
            child.position = newPos;
            Assert.That(child.localPosition, Is.Not.EqualTo(childLocal));
            Assert.That(child.position, Is.EqualTo(newPos));
        }
	
	
    }
}
