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

        [SetUp]
        public void Setup()
        {
            animation = new Components.Animation();
            h = new TestHierarchy();

            leftRight = new AnimationClip() { length = 1 };
            AnimationCurve c1 = new AnimationCurve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant };
            AnimationCurve c2 = new AnimationCurve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant };
            AnimationCurve c3 = new AnimationCurve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant };
            c1.Keys.Add(new CurveKey(0, 0, 10, 10, CurveContinuity.Smooth));
            c1.Keys.Add(new CurveKey(0.5f, 5, 0, 0, CurveContinuity.Smooth));
            c1.Keys.Add(new CurveKey(1, 0, 0, 0, CurveContinuity.Smooth));
            leftRight.curves = new AnimationClipCurveData[] { 
                new AnimationClipCurveData() { propertyName = "m_LocalPosition.x", curve = c1 },
                new AnimationClipCurveData() { propertyName = "m_LocalPosition.y", curve = c2 },
                new AnimationClipCurveData() { propertyName = "m_LocalPosition.z", curve = c3 }
            };
        }

        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void AnimationWillAlterTheTransforms()
        {
            h.root.AddComponent(animation);
            animation.AddClip(leftRight, "left/right");


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
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");

        }
	
    }
}
