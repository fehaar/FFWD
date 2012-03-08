using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Animation
{
    [TestFixture]
    public class WhenPlayingAClipQueued
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
        public void ItWillBeEnabledImmidiatelyIfNoOtherClipsArePlaying()
        {
            string name = "MyClip";
            AnimationClip clip = new AnimationClip();
            animation.AddClip(clip, name);

            AnimationState state = animation.PlayQueued(name);
            Assert.That(state.enabled, Is.True);
            Assert.That(state.playQueuedReference, Is.True);
            Assert.That(state, Is.Not.SameAs(animation[name]));
            Assert.That(animation[name].playQueuedReference, Is.False);
        }

        [Test]
        public void ItWillNotBeEnabledIfOtherClipsArePlaying()
        {
            string name = "MyClip";
            AnimationClip clip = new AnimationClip();
            animation.AddClip(clip, name);
            animation.Play(name);

            AnimationState state = animation.PlayQueued(name);
            Assert.That(state.enabled, Is.False);
        }

        [Test]
        public void TheQueuedClipWillBeStartedWhenTheRuningClipEnds()
        {
            string name = "MyClip";
            animation.AddClip(new AnimationClip() { length = 2, wrapMode = WrapMode.Once }, name);
            animation.Play(name);
            AnimationState state = animation.PlayQueued(name);
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation[name].enabled, Is.False);
            Assert.That(state.enabled, Is.True);
        }

        [Test]
        public void WhenTheQueuedAnimationHasEndedTheStateWillNotExistAnymore()
        {
            string name = "MyClip";
            animation.AddClip(new AnimationClip() { length = 2, wrapMode = WrapMode.Once }, name);
            AnimationState state = animation.PlayQueued(name);
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation[state.name], Is.Null);
        }
	
	
    }
}
