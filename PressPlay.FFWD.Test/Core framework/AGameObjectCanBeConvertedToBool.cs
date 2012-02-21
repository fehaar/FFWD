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
        public void ANewlyCreatedGameObjectWillBeTrue()
        {
            GameObject go = new GameObject();
            bool exists = go;
            Assert.That(exists, Is.True);
        }

        [Test]
        public void ADestroyedGameObjectWillBeFalse()
        {
            GameObject go = new GameObject();
            TestComponent cmp = go.AddComponent(new TestComponent());
            Assert.That((bool)cmp, Is.True);

            GameObject.Destroy(go);

            Application.CleanUp();

            Assert.That((bool)go, Is.False);
        }
	
	
    }
}
