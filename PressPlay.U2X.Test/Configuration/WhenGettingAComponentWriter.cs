using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PressPlay.U2X.Interfaces;

namespace PressPlay.U2X.Test.Configuration
{
    public class TestComponentWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(Writers.SceneWriter scene, object component)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    [TestFixture]
    public class WhenGettingAComponentWriter
    {
        TypeResolver resolver = new TypeResolver();

        [Test]
        public void WeWillGetAnInstanceOfTheWriterIfItIsConfigured()
        {
            resolver.ComponentWriters.Add(new ComponentMap() { Type = "System.String", To = typeof(TestComponentWriter).AssemblyQualifiedName });
            IComponentWriter writer = resolver.GetComponentWriter("Hello".GetType());
            Assert.That(writer, Is.Not.Null);
            Assert.That(writer, Is.InstanceOf<TestComponentWriter>());
        }

        [Test]
        public void WeWillGetNullIfAWriterIsNotConfigured()
        {
            IComponentWriter writer = resolver.GetComponentWriter("Hello".GetType());
            Assert.That(writer, Is.Null);
        }
	
    }
}
