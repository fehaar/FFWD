using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Configuration;
using System.Reflection;
using PressPlay.U2X.Configuration;
using System.Xml;
using System.IO;

namespace PressPlay.U2X
{
	public class TypeResolver
	{
		public TypeResolver()
		{
			ExcludeTypes = new List<string>();
			IncludeTypes = new List<string>();
			NamespaceRules = new List<NamespaceRule>();
			DefaultNamespace = "PressPlay.U2X.Xna";
		}

		[XmlAttribute]
		public bool ExcludeByDefault { get; set; }
		[XmlArray]
		[XmlArrayItem("Type", typeof(string))]
		public List<string> ExcludeTypes { get; set; }
		[XmlArray]
		[XmlArrayItem("Type", typeof(string))]
		public List<string> IncludeTypes { get; set; }
		[XmlElement]
		public string DefaultNamespace { get; set; }
		[XmlArray]
		[XmlArrayItem("Convert", typeof(NamespaceRule))]
		public List<NamespaceRule> NamespaceRules { get; set; }

		public bool SkipComponent(Object component)
		{
			String type = component.GetType().FullName;
			if (IncludeTypes.Contains(type))
			{
				return false;
			}
			if (ExcludeTypes.Contains(type))
			{
				return true;
			}
			return ExcludeByDefault;
		}

		public string ResolveTypeName(object component)
		{
            Type type = component.GetType();
            string result = type.FullName;
            foreach (NamespaceRule rule in NamespaceRules)
            {
                if (rule.Namespace == type.Namespace && !String.IsNullOrEmpty(rule.To))
                {
                    result = result.Replace(rule.Namespace, rule.To);
                }
            }
			return result;
		}	

		public static TypeResolver ReadConfiguration(string location)
		{
			XmlDocument doc = new XmlDocument();
			if (!File.Exists(location))
			{
				throw new FileNotFoundException("The configuration file does not exist", location);
			}
			doc.Load(location);
			XmlNode node = doc.SelectSingleNode("configuration/PressPlay/U2X");
			if (node == null)
			{
				throw new Exception("The configuration needs a node at the path configuration/PressPlay/U2X to read from.");
			}
			ConfigurationSectionHandler handler = new ConfigurationSectionHandler();
			return (TypeResolver)handler.Create(null, null, node);
		}
	}
}
