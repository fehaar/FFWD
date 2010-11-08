using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Configuration;
using PressPlay.U2X.Configuration;
using System.Reflection;

namespace PressPlay.U2X.Test.Configuration
{
	[TestFixture]
	public class WhenConfiguringTheTypeResolver
	{
        [Test]
		public void WeCanReadTheConfigurationSection()
		{
            TypeResolver obj = (TypeResolver)ConfigurationManager.GetSection("PressPlay/U2X");
            Assert.That(obj, Is.Not.Null);
            Assert.That(obj, Is.InstanceOf<TypeResolver>());

            Assert.That(obj.ExcludeByDefault, Is.True);
            Assert.That(obj.ExcludeTypes, Is.Not.Null);
            Assert.That(obj.ExcludeTypes, Is.Not.Empty);
            Assert.That(obj.IncludeTypes, Is.Not.Null);
            Assert.That(obj.IncludeTypes, Is.Not.Empty);

            Assert.That(obj.DefaultNamespace, Is.EqualTo("PressPlay.U2X"));

            Assert.That(obj.NamespaceRules, Is.Not.Null);
            Assert.That(obj.NamespaceRules, Is.Not.Empty);
            Assert.That(obj.NamespaceRules[0].Namespace, Is.EqualTo("System"));
            Assert.That(obj.NamespaceRules[0].To, Is.EqualTo("Testing"));
        }
	}
}
