using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PressPlay.FFWD.Exporter.Interfaces;
using PressPlay.FFWD.Exporter.Writers;

namespace PressPlay.FFWD.Exporter.Test.Configuration
{
    public class TestComponentWriter : IComponentWriter
    {
        #region IComponentWriter Members
        public void Write(SceneWriter scene, object component)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    public class TestFilteredComponentWriter : TestComponentWriter, IFilteredComponentWriter
    {
        public Filter filter { get; set; }
    }

    public class TestOptionComponentWriter : TestComponentWriter, IOptionComponentWriter
    {
        public string options { get; set; }
    }


    [TestFixture]
    public class WhenGettingAComponentWriter
    {
        TypeResolver resolver;

        [SetUp]
        public void Setup( )
        {
            resolver = new TypeResolver();
        }
	

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

        [Test]
        public void WeWillNotHaveAFilterIfOneIsNotConfigured()
        {
            resolver.ComponentWriters.Add(new ComponentMap() { Type = "System.String", To = typeof(TestFilteredComponentWriter).AssemblyQualifiedName });
            IFilteredComponentWriter writer = (IFilteredComponentWriter)resolver.GetComponentWriter("Hello".GetType());
            Assert.That(writer, Is.Not.Null);
            Assert.That(writer, Is.InstanceOf<TestFilteredComponentWriter>());
            Assert.That(writer.filter, Is.Null);
        }

        [Test]
        public void WeWillHaveAFilterIfOneIsConfigured()
        {
            resolver.ComponentWriters.Add(new ComponentMap() { Type = "System.String", To = typeof(TestFilteredComponentWriter).AssemblyQualifiedName, FilterType = Filter.FilterType.Exclude, FilterItems = "filter" });
            IFilteredComponentWriter writer = (IFilteredComponentWriter)resolver.GetComponentWriter("Hello".GetType());
            Assert.That(writer, Is.Not.Null);
            Assert.That(writer, Is.InstanceOf<TestFilteredComponentWriter>());
            Assert.That(writer.filter, Is.Not.Null);
            Assert.That(writer.filter.filterType, Is.EqualTo(Filter.FilterType.Exclude));
            Assert.That(writer.filter.items, Contains.Item("filter"));
        }

        [Test]
        public void WeWillNotHaveEmptyOptionsIfTheyAreNotDefined()
        {
            resolver.ComponentWriters.Add(new ComponentMap() { Type = "System.String", To = typeof(TestOptionComponentWriter).AssemblyQualifiedName });
            IOptionComponentWriter writer = (IOptionComponentWriter)resolver.GetComponentWriter("Hello".GetType());
            Assert.That(writer, Is.Not.Null);
            Assert.That(writer, Is.InstanceOf<TestOptionComponentWriter>());
            Assert.That(writer.options, Is.EqualTo(String.Empty));
        }

        [Test]
        public void WeWillHaveOptionsIfTheyAreDefined()
        {
            resolver.ComponentWriters.Add(new ComponentMap() { Type = "System.String", To = typeof(TestOptionComponentWriter).AssemblyQualifiedName, Options = "Test" });
            IOptionComponentWriter writer = (IOptionComponentWriter)resolver.GetComponentWriter("Hello".GetType());
            Assert.That(writer, Is.Not.Null);
            Assert.That(writer, Is.InstanceOf<TestOptionComponentWriter>());
            Assert.That(writer.options, Is.EqualTo("Test"));
        }
	
	
    }
}
