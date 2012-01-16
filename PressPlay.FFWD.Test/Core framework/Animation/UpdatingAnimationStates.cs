using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Animation
{
    [TestFixture]
    public class UpdatingAnimationStates
    {
        PressPlay.FFWD.Components.Animation animation;

        [SetUp]
        public void Setup()
        {
            animation = new Components.Animation();

            animation.AddClip(new AnimationClip() { length = 10 }, "Clip1");
            animation.AddClip(new AnimationClip() { length = 10 }, "Clip2");
            animation.AddClip(new AnimationClip() { length = 10 }, "Clip3");
        }

        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void WillUpdateTimeOnAnEnabledClip()
        {
            animation.Play("Clip1");
            Assert.That(animation["Clip1"].time, Is.EqualTo(0));

            animation.UpdateAnimationStates(1.0f);
            Assert.That(animation["Clip1"].time, Is.EqualTo(1.0f));
        }

        [Test]
        public void WillUpdateTimeOnAllEnabledClips()
        {
            animation.Play("Clip1");
            animation.Play("Clip2");

            animation.UpdateAnimationStates(1.0f);
            Assert.That(animation["Clip1"].time, Is.EqualTo(1.0f));
            Assert.That(animation["Clip2"].time, Is.EqualTo(1.0f));
        }

        [Test]
        public void WillNotUpdateTimeOnAnDisabledClip()
        {
            animation.UpdateAnimationStates(1.0f);
            Assert.That(animation["Clip1"].time, Is.EqualTo(0.0f));
        }

        [Test]
        public void WithSpeedSetWillUpdateTimeAccordingly()
        {
            animation.Play("Clip1");
            animation.Play("Clip2");
            animation.Play("Clip3");
            animation["Clip1"].speed = 2.0f;
            animation["Clip2"].speed = 0.5f;
            animation["Clip3"].speed = 0f;

            animation.UpdateAnimationStates(1.0f);
            Assert.That(animation["Clip1"].time, Is.EqualTo(2.0f));
            Assert.That(animation["Clip2"].time, Is.EqualTo(0.5f));
            Assert.That(animation["Clip3"].time, Is.EqualTo(0f));
        }

        [Test]
        public void CanMakeTheClipAnimateBackwards()
        {
            animation.Play("Clip1");
            animation.UpdateAnimationStates(1.0f);
            animation["Clip1"].speed = -1f;
            animation.UpdateAnimationStates(1.0f);

            Assert.That(animation["Clip1"].time, Is.EqualTo(0f));
        }	
    }
}
