using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Animation
{
    [TestFixture]
    public class WhenAddingANewClip
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
        public void TheClipWillGetAStateAndWeCanFindItAgain()
        {
            string name = "MyClip";
            AnimationClip clip = new AnimationClip();
            animation.AddClip(clip, name);
            Assert.That(animation[name], Is.Not.Null);
            Assert.That(animation.GetClip(name), Is.Not.Null);
            Assert.That(animation.GetClip(name), Is.SameAs(clip));
            Assert.That(animation.GetClipCount(), Is.EqualTo(1));
        }

        [Test]
	    public void WithANameThatAlreadyExistsWeWillReplaceTheOldClip()
	    {
            string name = "MyClip";
            AnimationClip clip = new AnimationClip();
            animation.AddClip(clip, name);
            AnimationClip newClip = new AnimationClip();
            animation.AddClip(newClip, name);
            Assert.That(animation[name], Is.Not.Null);
            Assert.That(animation.GetClip(name), Is.Not.Null);
            Assert.That(animation.GetClip(name), Is.SameAs(newClip));
            Assert.That(animation.GetClipCount(), Is.EqualTo(1));
	    }

        [Test]
        public void ItWillNotAutomaticallyBeSetAsTheDefaultClip()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");

        }
	

        [Test]
        public void ReplacingAnOldOneTheNewClipWillGetItsStateReset()
        {
            string name = "MyClip";
            AnimationClip clip = new AnimationClip();
            animation.AddClip(clip, name);
            AnimationState state = animation[name];

            AnimationClip newClip = new AnimationClip();
            animation.AddClip(newClip, name);

            Assert.That(animation[name], Is.Not.SameAs(state));
        }
    }
}
