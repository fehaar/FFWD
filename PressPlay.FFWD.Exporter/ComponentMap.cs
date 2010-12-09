using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace PressPlay.FFWD.Exporter
{
    public class ComponentMap
    {
        public ComponentMap()
        {
            FilterItems = String.Empty;
        }

        [XmlAttribute]
        public string Type { get; set; }
        [XmlAttribute]
        public string To { get; set; }
        [XmlAttribute]
        public Filter.FilterType FilterType { get; set; }
        [XmlAttribute]
        public string FilterItems { get; set; }
        [XmlAttribute]
        public string Options { get; set; }
    }
}
