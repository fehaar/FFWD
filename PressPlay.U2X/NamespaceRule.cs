using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PressPlay.U2X
{
    public class NamespaceRule
    {
        [XmlAttribute]
        public string Namespace { get; set; }
        [XmlAttribute]
        public string To { get; set; }
    }
}
