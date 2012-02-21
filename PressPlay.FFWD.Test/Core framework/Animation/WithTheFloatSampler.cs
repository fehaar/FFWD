using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Animation
{
    [TestFixture]
    public class WithTheFloatSampler
    {
        AnimationCurve x = new AnimationCurve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant };

        [SetUp]
        public void Setup()
        {
            x.Keys.Add(new CurveKey(0, 0, 10, 10, CurveContinuity.Smooth));
            x.Keys.Add(new CurveKey(0.5f, 5, 0, 0, CurveContinuity.Smooth));
            x.Keys.Add(new CurveKey(1, 0, 0, 0, CurveContinuity.Smooth));
        }

        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void WeCanChangeTheValueOfAPropertyOfAnObject()
        {
            SampleTester<float> test = new SampleTester<float>();
            FloatSampler sampler = new FloatSampler(test, "property", x);

            float oldValue = test.property;

            sampler.Sample(0.5f);

            Assert.That(test.property, Is.Not.EqualTo(oldValue));
        }

    }
}
