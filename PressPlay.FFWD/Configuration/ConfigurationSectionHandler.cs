using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;

namespace PressPlay.FFWD.Exporter.Configuration
{
    public class ConfigurationSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            XmlSerializer ser = new XmlSerializer(typeof(TypeResolver));
            TypeResolver config = null;
            using (XmlNodeReader rd = new XmlNodeReader(section.SelectSingleNode("TypeResolver")))
            {
                rd.Read();
                config = (TypeResolver)ser.Deserialize(rd);
            }
            return config;
        }

        #endregion
    }
}
