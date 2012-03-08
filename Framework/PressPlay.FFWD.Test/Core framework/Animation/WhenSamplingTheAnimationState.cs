using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Microsoft.Xna.Framework;
using PressPlay.FFWD.Components;

namespace PressPlay.FFWD.Test.Core_framework.Animation
{
    [TestFixture]
    public class WhenSamplingTheAnimationState
    {
        PressPlay.FFWD.Components.Animation animation;
        TestHierarchy h;

        AnimationClip leftRight;
        AnimationCurve simple;
        AnimationCurve empty;

        [SetUp]
        public void Setup()
        {
            animation = new Components.Animation();
            h = new TestHierarchy();

            leftRight = new AnimationClip() { length = 1 };
            simple = new AnimationCurve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant };
            empty = new AnimationCurve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant };
            simple.Keys.Add(new CurveKey(0, 0, 10, 10, CurveContinuity.Smooth));
            simple.Keys.Add(new CurveKey(0.5f, 5, 0, 0, CurveContinuity.Smooth));
            simple.Keys.Add(new CurveKey(1, 0, 0, 0, CurveContinuity.Smooth));
        }

        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void AnimationWillAlterTheTransforms()
        {
            leftRight.curves = new AnimationClipCurveData[] { 
                new AnimationClipCurveData() { propertyName = "m_LocalPosition.x", type = "PressPlay.FFWD.Transform", curve = simple },
                new AnimationClipCurveData() { propertyName = "m_LocalPosition.y", type = "PressPlay.FFWD.Transform", curve = empty },
                new AnimationClipCurveData() { propertyName = "m_LocalPosition.z", type = "PressPlay.FFWD.Transform", curve = empty }
            };
            animation.AddClip(leftRight, "left/right");
            h.root.AddComponent(animation);

            animation.Play("left/right");
            Vector3 position = h.rootTrans.localPosition;

            animation.UpdateAnimationStates(0.1f);
            animation.Sample();

            Assert.That(h.rootTrans.localPosition, Is.Not.EqualTo(position));
        }

        [Test]
        public void AnimationCanAlsoAlterPropertiesOnOtherComponentsThanTransform()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");

        }
	

        [Test]
        public void AnimationCanAlterThePathOfASubcomponent()
        {
            leftRight.curves = new AnimationClipCurveData[] { 
                new AnimationClipCurveData() { path = h.child.name, propertyName = "m_LocalPosition.x", type = "PressPlay.FFWD.Transform", curve = simple },
                new AnimationClipCurveData() { path = h.child.name + "/" + h.childOfChild.name, propertyName = "m_LocalPosition.y", type = "PressPlay.FFWD.Transform", curve = simple },
                new AnimationClipCurveData() { path = h.child.name, propertyName = "m_LocalPosition.z", type = "PressPlay.FFWD.Transform", curve = empty }
            };
            animation.AddClip(leftRight, "left/right");
            h.root.AddComponent(animation);

            animation.Play("left/right");
            Vector3 rootPosition = h.rootTrans.localPosition;
            Vector3 childPosition = h.childTrans.localPosition;
            Vector3 childOfChildPosition = h.childOfChildTrans.localPosition;

            animation.UpdateAnimationStates(0.1f);
            animation.Sample();

            Assert.That(h.rootTrans.localPosition, Is.EqualTo(rootPosition));
            Assert.That(h.childTrans.localPosition, Is.Not.EqualTo(childPosition));
            Assert.That(h.childOfChildTrans.localPosition, Is.Not.EqualTo(childOfChildPosition));
        }

        [Test]
        public void ItCanBeDoneByManuallySettingTheClipPositions()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");

        }
	
    }
}
