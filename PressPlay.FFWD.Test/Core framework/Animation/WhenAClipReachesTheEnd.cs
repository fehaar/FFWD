using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Animation
{
    [TestFixture]
    public class WhenAClipReachesTheEnd
    {
        PressPlay.FFWD.Components.Animation animation;

        [SetUp]
        public void Setup()
        {
            animation = new Components.Animation();

            animation.AddClip(new AnimationClip() { length = 2 }, "Clip1");
            animation.Play("Clip1");
        }

        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void WrapModeOnceWillStopTheClip()
        {
            animation["Clip1"].wrapMode = WrapMode.Once;
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation["Clip1"].time, Is.EqualTo(animation["Clip1"].length));
            Assert.That(animation["Clip1"].enabled, Is.False);
        }

        [Test]
        public void WrapModeOnceWillStopTheClipAlsoWhenRunningBackwards()
        {
            animation["Clip1"].wrapMode = WrapMode.Once;
            animation["Clip1"].time = animation["Clip1"].length;
            animation["Clip1"].speed = -1;
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation["Clip1"].time, Is.EqualTo(0));
            Assert.That(animation["Clip1"].enabled, Is.False);
        }

        [Test]
        public void WrapModeLoopWillLoopTheTime()
        {
            animation["Clip1"].wrapMode = WrapMode.Loop;
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation["Clip1"].time, Is.AtMost(0.21f));
            Assert.That(animation["Clip1"].enabled, Is.True);
        }

        [Test]
        public void WrapModeLoopWillLoopTheTimeAlsoWhenRunningBackwards()
        {
            animation["Clip1"].wrapMode = WrapMode.Loop;
            animation["Clip1"].time = animation["Clip1"].length;
            animation["Clip1"].speed = -1;
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation["Clip1"].time, Is.AtLeast(1.8f));
            Assert.That(animation["Clip1"].enabled, Is.True);
        }

        [Test]
        public void WrapModePingPongWillReverseTime()
        {
            animation["Clip1"].wrapMode = WrapMode.PingPong;
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation["Clip1"].time, Is.AtLeast(1.8f));
            Assert.That(animation["Clip1"].speed, Is.EqualTo(-1f));
            Assert.That(animation["Clip1"].enabled, Is.True);

            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation["Clip1"].time, Is.AtLeast(0.0f));
            Assert.That(animation["Clip1"].time, Is.AtMost(0.41f));
            Assert.That(animation["Clip1"].speed, Is.EqualTo(1f));
        }

        [Test]
        public void WrapModeClampForeverWillKeepTheClipAtTheEnd()
        {
            animation["Clip1"].wrapMode = WrapMode.Clamp;
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation["Clip1"].time, Is.EqualTo(animation["Clip1"].length));
            Assert.That(animation["Clip1"].enabled, Is.True);
            animation.UpdateAnimationStates(0.55f);
            Assert.That(animation["Clip1"].time, Is.EqualTo(animation["Clip1"].length));
            Assert.That(animation["Clip1"].enabled, Is.True);
        }

        [Test]
        public void WrapModeClampForeverWillKeepTheClipAtTheBegiinigWhenRunningBackwards()
        {
            animation["Clip1"].wrapMode = WrapMode.Clamp;
            animation["Clip1"].time = animation["Clip1"].length;
            animation["Clip1"].speed = -1;
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation["Clip1"].time, Is.EqualTo(0));
            Assert.That(animation["Clip1"].enabled, Is.True);
            animation.UpdateAnimationStates(0.55f);
            Assert.That(animation["Clip1"].time, Is.EqualTo(0));
            Assert.That(animation["Clip1"].enabled, Is.True);
        }

        [Test]
        public void WrapModeDefaultWillTakeTheWrapModeFromTheAnimationComponent()
        {
            animation["Clip1"].wrapMode = WrapMode.Default;
            animation.wrapMode = WrapMode.Loop;
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation["Clip1"].time, Is.AtMost(0.21f));
            Assert.That(animation["Clip1"].enabled, Is.True);
        }

        [Test]
        public void WrapModeDefaultOnBothStateAndComponentWillTakeTheWrapModeFromTheClip()
        {
            animation["Clip1"].wrapMode = WrapMode.Default;
            animation.wrapMode = WrapMode.Default;
            animation.GetClip("Clip1").wrapMode = WrapMode.Loop;
            for (int i = 0; i < 4; i++)
            {
                animation.UpdateAnimationStates(0.55f);
            }
            Assert.That(animation["Clip1"].time, Is.AtMost(0.21f));
            Assert.That(animation["Clip1"].enabled, Is.True);
        }
    }
}
