using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Configuration;
using System.Reflection;
using PressPlay.FFWD.Exporter.Configuration;
using System.Xml;
using System.IO;
using PressPlay.FFWD.Exporter.Interfaces;
using PressPlay.FFWD.Exporter.Writers;
using UnityEngine;

namespace PressPlay.FFWD.Exporter
{
	public class TypeResolver
	{
		public TypeResolver()
		{
			ExcludeTypes = new List<string>();
			IncludeTypes = new List<string>();
			NamespaceRules = new List<NamespaceRule>();
			DefaultNamespace = "PressPlay.FFWD";
            ComponentWriters = new List<ComponentMap>();
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
        [XmlArray]
        [XmlArrayItem("Map", typeof(ComponentMap))]
        public List<ComponentMap> ComponentWriters { get; set; }

        public static TypeResolver ReadConfiguration(string location)
        {
            XmlDocument doc = new XmlDocument();
            if (!File.Exists(location))
            {
                throw new FileNotFoundException("The configuration file does not exist", location);
            }
            doc.Load(location);
            XmlNode node = doc.SelectSingleNode("configuration/PressPlay/FFWD");
            if (node == null)
            {
                throw new Exception("The configuration needs a node at the path configuration/PressPlay/FFWD to read from.");
            }
            ConfigurationSectionHandler handler = new ConfigurationSectionHandler();
            return (TypeResolver)handler.Create(null, null, node);
        }

		public bool SkipComponent(object component)
		{
			String type = component.GetType().FullName;
			if (IncludeTypes != null && IncludeTypes.Contains(type))
			{
				return false;
			}
			if (ExcludeTypes != null && ExcludeTypes.Contains(type))
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
                if (!String.IsNullOrEmpty(rule.Namespace) && rule.Namespace == type.Namespace && !String.IsNullOrEmpty(rule.To))
                {
                    result = result.Replace(rule.Namespace, rule.To);
                }
                if (!String.IsNullOrEmpty(rule.Type) && rule.Type == result && !String.IsNullOrEmpty(rule.To))
                {
                    result = rule.To;
                }
            }
            if (component is MonoBehaviour && !result.Contains("."))
            {
                result = ScriptTranslator.ScriptNamespace + "." + result;
            }
            return result;
		}	

        public IComponentWriter GetComponentWriter(Type type)
        {
            string result = type.FullName;
            foreach (ComponentMap map in ComponentWriters)
            {
                try
                {
                    if (map.Type == result)
                    {
                        Type tp = Type.GetType(map.To);
                        IComponentWriter writer = (IComponentWriter)Activator.CreateInstance(tp);
                        if (writer is IFilteredComponentWriter)
                        {
                            (writer as IFilteredComponentWriter).filter = new Filter() { filterType = map.FilterType, items = map.FilterItems.Split(',') };
                        }
                        if (writer is IOptionComponentWriter)
                        {
                            (writer as IOptionComponentWriter).options = map.Options ?? String.Empty;
                        }

                        return writer;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Could create writer using map from " + map.Type + " to " + map.To + ". " + ex.Message);
                }
            }
            return null;
        }
    }
}
