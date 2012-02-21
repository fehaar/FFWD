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

        [Test]
        public void OtherRunningClipsWillBeStopped()
        {
            string name1 = "MyClip";
            animation.AddClip(new AnimationClip(), name1);
            string name2 = "MyNewClip";
            animation.AddClip(new AnimationClip(), name2);

            animation.Play(name1);
            Assert.That(animation.Play(name2), Is.True);
            Assert.That(animation.IsPlaying(name1), Is.False);
            Assert.That(animation.IsPlaying(name2), Is.True);
        }
	
    }
}
