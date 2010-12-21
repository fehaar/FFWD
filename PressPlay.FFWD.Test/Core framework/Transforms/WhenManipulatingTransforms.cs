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
        public void WeWillSetTheGameObjectOnANewTransform()
        {
            GameObject go = new GameObject();
            go.AddComponent(new Transform());
            Assert.That(go.transform.gameObject, Is.EqualTo(go));
        }

        [Test]
        public void AChildWillBePositionedRelativeToItsParent()
        {
            Transform trans = new Transform() { localPosition = new Vector3(2, 2, 2) };
            Transform child = new Transform() { localPosition = new Vector3(3, 2, 1) };
            GameObject childObj = new GameObject();
            childObj.AddComponent(child);

            Assert.That(trans.position, Is.EqualTo(new Vector3(2, 2, 2)));
            Assert.That(child.position, Is.EqualTo(new Vector3(3, 2, 1)));
            child.parent = trans;
            Assert.That(child.position, Is.EqualTo(new Vector3(3, 2, 1)));
        }

        [Test]
        public void MovingAParentWillMoveItsChild()
        {
            Transform trans = new Transform() { localPosition = Vector3.one };
            Transform child = new Transform() { localPosition = Vector3.up };
            GameObject childObj = new GameObject();
            childObj.AddComponent(child);

            child.parent = trans;
            Assert.That(child.position, Is.EqualTo(Vector3.up));            
            trans.localPosition = new Vector3(2, 2, 2);
            Assert.That(child.position, Is.EqualTo(new Vector3(2, 2, 2) + child.localPosition));
        }

        [Test]
        public void MovingAParentWillMoveAllChildren()
        {
            Transform trans = new Transform() { localPosition = Vector3.one };
            Transform child = new Transform() { localPosition = Vector3.up };
            Transform child2 = new Transform() { localPosition = Vector3.up };
            GameObject childObj = new GameObject();
            childObj.AddComponent(child);
            GameObject childObj2 = new GameObject();
            childObj2.AddComponent(child2);

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
            Transform trans = new Transform() { localScale = new Vector3(2, 2, 2) };
            Transform child = new Transform() { localScale = new Vector3(3, 2, 1) };
            GameObject childObj = new GameObject();
            childObj.AddComponent(child);

            Assert.That(trans.lossyScale, Is.EqualTo(new Vector3(2, 2, 2)));
            Assert.That(child.lossyScale, Is.EqualTo(new Vector3(3, 2, 1)));
            child.parent = trans;
            Assert.That(child.lossyScale, Is.EqualTo(new Vector3(2, 2, 2) * new Vector3(3, 2, 1)));
        }

        [Test]
        public void ScalingAParentWillScaleTheChild()
        {
            Transform trans = new Transform() { localScale = Vector3.one };
            Transform child = new Transform() { localScale = new Vector3(3, 2, 1) };
            GameObject childObj = new GameObject();
            childObj.AddComponent(child);

            child.parent = trans;
            Assert.That(child.lossyScale, Is.EqualTo(new Vector3(3, 2, 1)));
            trans.localScale = new Vector3(2, 2, 2);
            Assert.That(child.lossyScale, Is.EqualTo(new Vector3(2, 2, 2) * new Vector3(3, 2, 1)));
        }

        [Test]
        public void RotatingAParentWillRotateTheChild()
        {
            Transform trans = new Transform() { localRotation = Quaternion.Identity };
            Transform child = new Transform() { localRotation = Quaternion.Identity };
            GameObject childObj = new GameObject();
            childObj.AddComponent(child);

            Assert.That(trans.rotation, Is.EqualTo(Quaternion.Identity));
            Assert.That(child.rotation, Is.EqualTo(Quaternion.Identity));
            child.parent = trans;
            trans.localRotation = Quaternion.Euler(MathHelper.PiOver2, 0.0f, 0.0f);
            Assert.That(child.rotation, Is.Not.EqualTo(Quaternion.Identity));
        }

        [Test]
        public void RotatingAParentWillMoveATranslatedChild()
        {
            Transform trans = new Transform() { localPosition = new Vector3(2, 2, 2) };
            Transform child = new Transform() { localPosition = new Vector3(3, 2, 1) };
            GameObject childObj = new GameObject();
            childObj.AddComponent(child);

            Assert.That(trans.rotation, Is.EqualTo(Quaternion.Identity));
            Assert.That(child.rotation, Is.EqualTo(Quaternion.Identity));
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
            Transform trans = new Transform() { localPosition = new Vector3(2, 2, 2) };
            Vector3 childLocal = new Vector3(3, 2, 1);
            Transform child = new Transform() { localPosition = childLocal };
            GameObject childObj = new GameObject();
            childObj.AddComponent(child);
            child.parent = trans;

            Vector3 newPos = childLocal + new Vector3(10, 10, 10);
            child.position = newPos;
            Assert.That(child.localPosition, Is.Not.EqualTo(childLocal));
            Assert.That(child.position, Is.EqualTo(newPos));
        }
	
	
    }
}
