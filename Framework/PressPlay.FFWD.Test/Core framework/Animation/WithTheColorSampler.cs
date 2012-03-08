using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PressPlay.FFWD.Components;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Animation
{
    [TestFixture]
    public class WithTheColorSampler
    {
        AnimationCurve r = new AnimationCurve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant };

        [SetUp]
        public void Setup()
        {
            r.Keys.Add(new CurveKey(0, 0, 10, 10, CurveContinuity.Smooth));
            r.Keys.Add(new CurveKey(0.5f, 5, 0, 0, CurveContinuity.Smooth));
            r.Keys.Add(new CurveKey(1, 0, 0, 0, CurveContinuity.Smooth));
        }

        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void WeCanChangeTheValueOfAPropertyOfAnObject()
        {
            SampleTester<Color> test = new SampleTester<Color>() { property = new Color(0.5f, 0.5f, 0.5f, 0.5f) };
            ColorSampler sampler = new ColorSampler(test, "property", r, null, null, null);

            Color oldValue = test.property;

            sampler.Sample(0.5f);

            Assert.That(test.property.r, Is.Not.EqualTo(oldValue.r));
            Assert.That(test.property.g, Is.EqualTo(oldValue.g));
            Assert.That(test.property.b, Is.EqualTo(oldValue.b));
            Assert.That(test.property.a, Is.EqualTo(oldValue.a));
        }
    }
}
