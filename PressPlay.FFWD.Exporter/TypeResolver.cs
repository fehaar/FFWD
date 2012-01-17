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
using PressPlay.FFWD.Exporter.Writers.Components;

namespace PressPlay.FFWD.Exporter
{
	public class TypeResolver
	{
		public TypeResolver()
		{
			NamespaceRules = new List<NamespaceRule>();
			DefaultNamespace = "PressPlay.FFWD";
            ComponentWriters = new List<ComponentMap>();
		}

		[XmlAttribute]
		public bool ExcludeByDefault { get; set; }
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
            if (!(component is MonoBehaviour))
            {
                return false;
            }
            if (ExcludeByDefault)
            {
                Type tp = component.GetType();
                foreach (var item in tp.GetCustomAttributes(true))
                {
                    if (item.GetType().Name == "FFWD_ExportOptionsAttribute")
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                Type tp = component.GetType();
                foreach (var item in tp.GetCustomAttributes(true))
                {
                    if (item.GetType().Name == "FFWD_DontExportAttribute")
                    {
                        return true;
                    }
                }
                return false;
            }
		}

        public string ResolveTypeName(string fullName)
        {
            int index = fullName.LastIndexOf('.');
            string ns = (index > 0) ? fullName.Substring(0, index) : String.Empty;
            string result = fullName;
            foreach (NamespaceRule rule in NamespaceRules)
            {
                if (!String.IsNullOrEmpty(rule.Namespace) && rule.Namespace == ns && !String.IsNullOrEmpty(rule.To))
                {
                    result = result.Replace(rule.Namespace, rule.To);
                }
                if (!String.IsNullOrEmpty(rule.Type) && rule.Type == result && !String.IsNullOrEmpty(rule.To))
                {
                    result = rule.To;
                }
            }
            return result;
        }

		public string ResolveObjectType(object component)
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
                if (!String.IsNullOrEmpty(ScriptTranslator.ScriptNamespace))
                {
                    result = ScriptTranslator.ScriptNamespace + "." + result;
                }
            }
            return result;
		}	

        public IComponentWriter GetComponentWriter(Type type)
        {
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                MonoBehaviourWriter wr = new MonoBehaviourWriter();
                wr.filter = new Filter() { filterType = wr.defaultFilterType };
                wr.options = "noExport";
                return wr;
            }

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
                            if (map.FilterType == Filter.FilterType.None)
                            {
                                (writer as IFilteredComponentWriter).filter = new Filter() { filterType = (writer as IFilteredComponentWriter).defaultFilterType };
                            }
                            else
                            {
                                (writer as IFilteredComponentWriter).filter = new Filter() { filterType = map.FilterType, items = map.FilterItems.Split(',') };
                            }
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
