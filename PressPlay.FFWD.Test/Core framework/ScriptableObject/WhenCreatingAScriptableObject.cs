using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Test.Core_framework
{
    [TestFixture]
    public class WhenCreatingAScriptableObject
    {
        [Test]
        public void WeCanDoItByType()
        {
            ScriptableObject o = ScriptableObject.CreateInstance(typeof(TestScriptableObject));
            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf<TestScriptableObject>());
        }

        [Test]
        public void WeCanDoItByGenerics()
        {
            TestScriptableObject o = ScriptableObject.CreateInstance<TestScriptableObject>();
            Assert.That(o, Is.Not.Null);
        }

        [Test]
        public void WeCanDoItByName()
        {
            ScriptableObject o = ScriptableObject.CreateInstance("PressPlay.FFWD.Test.Core_framework.TestScriptableObject");
            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf<TestScriptableObject>());
        }
	
	
    }
}
