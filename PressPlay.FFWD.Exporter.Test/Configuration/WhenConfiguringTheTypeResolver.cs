using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Configuration;
using PressPlay.FFWD.Exporter.Configuration;
using System.Reflection;
using PressPlay.FFWD.Exporter.Writers.Components;

namespace PressPlay.FFWD.Exporter.Test.Configuration
{
	[TestFixture]
	public class WhenConfiguringTheTypeResolver
	{
        [Test]
		public void WeCanReadTheConfigurationSection()
		{
            TypeResolver obj = (TypeResolver)ConfigurationManager.GetSection("PressPlay/FFWD");
            Assert.That(obj, Is.Not.Null);
            Assert.That(obj, Is.InstanceOf<TypeResolver>());

            Assert.That(obj.ExcludeByDefault, Is.True);
            Assert.That(obj.ExcludeTypes, Is.Not.Null);
            Assert.That(obj.ExcludeTypes, Is.Not.Empty);
            Assert.That(obj.IncludeTypes, Is.Not.Null);
            Assert.That(obj.IncludeTypes, Is.Not.Empty);

            Assert.That(obj.DefaultNamespace, Is.EqualTo("PressPlay.FFWD"));

            Assert.That(obj.NamespaceRules, Is.Not.Null);
            Assert.That(obj.NamespaceRules, Is.Not.Empty);
            Assert.That(obj.NamespaceRules[0].Namespace, Is.EqualTo("System"));
            Assert.That(obj.NamespaceRules[0].To, Is.EqualTo("Testing"));

            Assert.That(obj.ComponentWriters, Is.Not.Null);
            Assert.That(obj.ComponentWriters, Is.Not.Empty);
            ComponentMap map = obj.ComponentWriters.Find(m => m.Type == "UnityEngine.MeshRenderer");
            Assert.That(map, Is.Not.Null);
            Assert.That(map.To, Is.StringStarting(typeof(MeshRendererWriter).FullName));
            Assert.That(map.FilterType, Is.EqualTo(Filter.FilterType.ExcludeAll));

            map = obj.ComponentWriters.Find(m => m.Type == "AnimatedCheckPoint");
            Assert.That(map, Is.Not.Null);
            Assert.That(map.FilterType, Is.EqualTo(Filter.FilterType.Exclude));
            Assert.That(map.FilterItems, Is.EqualTo("sndActivate,challenge"));
        }
	}
}
