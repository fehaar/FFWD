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
            Assert.Ignore("We have changed the wayt that this works, so the tests need to be rewritten");

            resolver = new TypeResolver() { ExcludeByDefault = true };
            Assert.That(resolver.SkipComponent("Test"), Is.True);
            Assert.That(resolver.SkipComponent(1), Is.True);
        }

        [Test]
        public void WeWillNotSkipAnyComponentIfExcludeByDefaultIsFalse()
        {
            Assert.Ignore("We have changed the wayt that this works, so the tests need to be rewritten");

            resolver = new TypeResolver() { ExcludeByDefault = false };
            Assert.That(resolver.SkipComponent("Test"), Is.False);
            Assert.That(resolver.SkipComponent(1), Is.False);
        }
    }
}
