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
    public class WithTheVector3Sampler
    {
        AnimationCurve x = new AnimationCurve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant };
        AnimationCurve y = new AnimationCurve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant };
        AnimationCurve z = new AnimationCurve() { PreLoop = CurveLoopType.Constant, PostLoop = CurveLoopType.Constant };

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
            SampleTester<Vector3> test = new SampleTester<Vector3>();
            Vector3Sampler sampler = new Vector3Sampler(test, "property", x, y, z);

            Vector3 oldValue = test.property;

            sampler.Sample(0.5f);

            Assert.That(test.property, Is.Not.EqualTo(oldValue));
        }

        [Test]
        public void WeCanChangeTheValueOfAFieldOfAnObject()
        {
            SampleTester<Vector3> test = new SampleTester<Vector3>();
            Vector3Sampler sampler = new Vector3Sampler(test, "field", x, y, z);

            Vector3 oldValue = test.property;

            sampler.Sample(0.5f);

            Assert.That(test.field, Is.Not.EqualTo(oldValue));
        }

        [Test]
        public void WeCanSampleOnAPrivateField()
        {
            SampleTester<Vector3> test = new SampleTester<Vector3>();
            Vector3Sampler sampler = new Vector3Sampler(test, "privateField", x, y, z);
            Vector3 oldValue = test.property;

            sampler.Sample(0.5f);

            Assert.That(test.GetPrivateField(), Is.Not.EqualTo(oldValue));
        }

        [Test]
        public void WeCanSampleWithFewerCurvesWithoutDestroyingExistingValues()
        {
            SampleTester<Vector3> test = new SampleTester<Vector3>() { property = new Vector3(3) };
            Vector3Sampler sampler = new Vector3Sampler(test, "property", x, null, null);

            Vector3 oldValue = test.property;

            sampler.Sample(0.5f);

            Assert.That(test.property.x, Is.Not.EqualTo(oldValue.x));
            Assert.That(test.property.y, Is.EqualTo(oldValue.y));
            Assert.That(test.property.z, Is.EqualTo(oldValue.z));
        }
	
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WeWillGetAnErrorIfWeTryToSampleAMethod()
        {
            SampleTester<Vector3> test = new SampleTester<Vector3>();
            Vector3Sampler sampler = new Vector3Sampler(test, "Method", x, y, z);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void WeWillGetAnErrorIfWeTryToCreateASamplerForANonExistingMember()
        {
            SampleTester<Vector3> test = new SampleTester<Vector3>();
            Vector3Sampler sampler = new Vector3Sampler(test, "NonExistantMember", x, y, z);
        }
	
    }
}
