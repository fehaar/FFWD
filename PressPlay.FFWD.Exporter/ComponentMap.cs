using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PressPlay.FFWD.Exporter
{
    public class ComponentMap
    {
        [XmlAttribute]
        public string Type { get; set; }
        [XmlAttribute]
        public string To { get; set; }
    }
}
