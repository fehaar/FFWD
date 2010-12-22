using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class AGameObjectCanBeConvertedToBool
    {
        [TearDown]
        public void TearDown()
        {
            Application.Reset();
        }

        [Test]
        public void AnExistingGameObjectWillBeTrue()
        {
            GameObject go = new GameObject();
            Application.AwakeNewComponents();
            bool exists = go;
            Assert.That(exists, Is.True);
        }

        [Test]
        public void ADisconnectedGameObjectWillBeFalse()
        {
            GameObject go = new GameObject();
            bool exists = go;
            Assert.That(exists, Is.False);
        }

        [Test]
        public void ADestroyedGameObjectWillBeFalse()
        {
            // TODO : Add implementation of test
            Assert.Ignore("Test not implemented");

        }
	
	
    }
}
