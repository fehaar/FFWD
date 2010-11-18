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
        TypeResolver resolver;

        [SetUp]
        public void Setup( )
        {
            resolver = new TypeResolver();
        }

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

        [Test]
        public void ATypeConversionRuleWillChangeTheType()
        {
            resolver.NamespaceRules.Add(new NamespaceRule() { Type = "System.String", To = "Testing.MyString" });
            Assert.That(resolver.ResolveTypeName("Test"), Is.EqualTo("Testing.MyString"));
        }

        [Test]
        public void ATypeConversionRuleWillNotChangeOtherTypes()
        {
            resolver.NamespaceRules.Add(new NamespaceRule() { Type = "System.String", To = "Testing.MyString" });
            Assert.That(resolver.ResolveTypeName(1), Is.EqualTo("System.Int32"));
        }
	
        [Test]
        public void ANullNamespaceWillNotCauseAProblem()
        {
            resolver.NamespaceRules.Add(new NamespaceRule() { Type = "System.String", To = "Testing.MyString" });
            Assert.That(resolver.ResolveTypeName(new NoNamespaceType()), Is.EqualTo("NoNamespaceType"));
        }
	
    }
}

class NoNamespaceType { }