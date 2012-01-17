using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Animation
{
    [TestFixture]
    public class WhenPlayingAClip
    {
        PressPlay.FFWD.Components.Animation animation;

        [SetUp]
        public void Setup()
        {
            animation = new Components.Animation();
        }

        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void PlayWillReturnFalseIfTheClipIsNotPresent()
        {
            Assert.That(animation.Play("MyClip"), Is.False);
        }

        [Test]
        public void PlayWillReturnFalseIfTheDefaultClipIsNotPresent()
        {
            Assert.That(animation.Play(), Is.False);
        }

        [Test]
        public void PlayWillReturnTrueAndEnableTheClipIfPresent()
        {
            string name = "MyClip";
            AnimationClip clip = new AnimationClip();
            animation.AddClip(clip, name);

            Assert.That(animation.Play(name), Is.True);
            Assert.That(animation.IsPlaying(name), Is.True);
            Assert.That(animation.isPlaying, Is.True);
        }
    }
}
