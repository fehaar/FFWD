using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace PressPlay.FFWD.Exporter.Test.Configuration
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
            Assert.That(resolver.ResolveObjectType("Test"), Is.EqualTo("Test".GetType().FullName));
            Assert.That(resolver.ResolveTypeName("System.String"), Is.EqualTo("System.String"));
        }

        [Test]
        public void ANamespaceConversionRuleWillChangeTheNamespace()
        {
            resolver.NamespaceRules.Add(new NamespaceRule() { Namespace = "System", To = "Testing" });
            Assert.That(resolver.ResolveObjectType("Test"), Is.EqualTo("Testing.String"));
            Assert.That(resolver.ResolveTypeName("System.String"), Is.EqualTo("Testing.String"));
        }

        [Test]
        public void ATypeConversionRuleWillChangeTheType()
        {
            resolver.NamespaceRules.Add(new NamespaceRule() { Type = "System.String", To = "Testing.MyString" });
            Assert.That(resolver.ResolveObjectType("Test"), Is.EqualTo("Testing.MyString"));
            Assert.That(resolver.ResolveTypeName("System.String"), Is.EqualTo("Testing.MyString"));
        }

        [Test]
        public void ATypeConversionRuleWillNotChangeOtherTypes()
        {
            resolver.NamespaceRules.Add(new NamespaceRule() { Type = "System.String", To = "Testing.MyString" });
            Assert.That(resolver.ResolveObjectType(1), Is.EqualTo("System.Int32"));
            Assert.That(resolver.ResolveTypeName("System.Int32"), Is.EqualTo("System.Int32"));
        }
	
        [Test]
        public void ANullNamespaceWillNotCauseAProblem()
        {
            resolver.NamespaceRules.Add(new NamespaceRule() { Type = "System.String", To = "Testing.MyString" });
            Assert.That(resolver.ResolveObjectType(new NoNamespaceType()), Is.EqualTo("NoNamespaceType"));
            Assert.That(resolver.ResolveTypeName(new NoNamespaceType().GetType().FullName), Is.EqualTo("NoNamespaceType"));
        }
	
    }
}

class NoNamespaceType { }