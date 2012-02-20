using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework.Behaviours
{
    [TestFixture]
    public class WhenSettingTheEnabledProperty : ApplicationTestBase
    {
        TestBehaviour behaviour;
        bool hasUpdated;

        [SetUp]
        public void CreateTheBehaviour()
        {
            GameObject go = new GameObject();
            behaviour = go.AddComponent<TestBehaviour>();
            hasUpdated = false;
        }

        [Test]
        public void SettingItToTrueWillUpdateTheBehaviourOnUpdate()
        {
            behaviour.enabled = true;
            behaviour.onUpdate = () => { hasUpdated = true; };
            Assert.Inconclusive("We cannot run an application loop from test, so we cannot see if this works. Bummer.");
            Assert.That(hasUpdated, Is.True);
        }

        [Test]
        public void SettingItToFalseWillUpdateTheBehaviourOnUpdate()
        {
            behaviour.enabled = false;
            behaviour.onUpdate = () => { hasUpdated = true; };
            Assert.Inconclusive("We cannot run an application loop from test, so we cannot see if this works. Bummer.");
            Assert.That(hasUpdated, Is.False);
        }
    }
}
