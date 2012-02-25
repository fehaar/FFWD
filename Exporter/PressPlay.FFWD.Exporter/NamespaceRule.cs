using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PressPlay.FFWD.Exporter
{
    public class NamespaceRule
    {
        [XmlAttribute]
        public string Namespace { get; set; }
        [XmlAttribute]
        public string Type { get; set; }
        [XmlAttribute]
        public string To { get; set; }
    }
}
