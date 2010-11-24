using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Exporter.Test.Configuration
{
    [TestFixture]
    public class WhenCheckingIfAComponentShouldBeExported
    {
        TypeResolver resolver;

        [Test]
        public void WeWillSkipAllComponentsIfExcludeByDefaultIsTrue()
        {
            resolver = new TypeResolver() { ExcludeByDefault = true };
            Assert.That(resolver.SkipComponent("Test"), Is.True);
            Assert.That(resolver.SkipComponent(1), Is.True);
        }

        [Test]
        public void WeWillNotSkipAnyComponentIfExcludeByDefaultIsFalse()
        {
            resolver = new TypeResolver() { ExcludeByDefault = false };
            Assert.That(resolver.SkipComponent("Test"), Is.False);
            Assert.That(resolver.SkipComponent(1), Is.False);
        }

        [Test]
        public void WeCanIncludeSpecificComponents()
        {
            string[] types = { "System.String" };
            resolver = new TypeResolver() { ExcludeByDefault = true, IncludeTypes = new List<string>(types) };
            Assert.That(resolver.SkipComponent("Test"), Is.False);
            Assert.That(resolver.SkipComponent(1), Is.True);
        }

        [Test]
        public void WeCanExcludeSpecificTypes()
        {
            string[] types = { "System.Int32" };
            resolver = new TypeResolver() { ExcludeByDefault = false, ExcludeTypes = new List<string>(types) };
            Assert.That(resolver.SkipComponent("Test"), Is.False);
            Assert.That(resolver.SkipComponent(1), Is.True);
        }
    }
}
