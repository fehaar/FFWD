using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Animation
{
    [TestFixture]
    public class TheDefaultAnimationClip
    {
        PressPlay.FFWD.Components.Animation animation;
        string name = "MyClip";
        AnimationClip clip;

        [SetUp]
        public void Setup()
        {
            animation = new Components.Animation();
            clip = new AnimationClip() { name = name };
            animation.clip = clip;
        }

        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void CanBeSetWithTheClipProperty()
        {
            Assert.That(animation.clip, Is.Not.Null);
            Assert.That(animation.clip, Is.SameAs(clip));
            Assert.That(animation[name], Is.Not.Null);
            Assert.That(animation.GetClip(name), Is.Not.Null);
            Assert.That(animation.GetClip(name), Is.SameAs(clip));
            Assert.That(animation.GetClipCount(), Is.EqualTo(1));
        }

        [Test]
        public void WillBePlayedAutomaticallyOnAwakeIfPlayAutomaticallyIsSet()
        {
            Assert.That(animation.playAutomatically, Is.False);
            Assert.That(animation.isPlaying, Is.False);
            Assert.That(animation[name].enabled, Is.False);
            Assert.That(animation.IsPlaying(name), Is.False);

            animation.playAutomatically = true;
            animation.Awake();

            Assert.That(animation.isPlaying, Is.True);
            Assert.That(animation[name].enabled, Is.True);
            Assert.That(animation.IsPlaying(name), Is.True);
        }

        [Test]
        public void WillNotBePlayedAutomaticallyOnAwakeIfPlayAutomaticallyIsNotSet()
        {
            animation.Awake();

            Assert.That(animation.isPlaying, Is.False);
            Assert.That(animation[name].enabled, Is.False);
            Assert.That(animation.IsPlaying(name), Is.False);
        }
	
    }
}
