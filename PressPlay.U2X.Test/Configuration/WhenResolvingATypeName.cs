using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.U2X.Test.Configuration
{
    [TestFixture]
    public class WhenResolvingATypeName
    {
        TypeResolver resolver = new TypeResolver();

        [Test]
        public void TheResolvedNameWillBeTheFullNameOfTheType()
        {
            Assert.That(resolver.ResolveTypeName("Test"), Is.EqualTo("Test".GetType().FullName));
        }

        [Test]
        public void ANamespaceConversionRuleWillChangeTheNamespace()
        {
            resolver.NamespaceRules.Add(new NamespaceRule() { Namespace = "System", To = "Testing" });
            Assert.That(resolver.ResolveTypeName("Test"), Is.EqualTo("Testing.String"));
        }
	
	
    }
}
