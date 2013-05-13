using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DeplyScriptTest
{
    /// <summary>
    /// Provides a set of utilities to read xml/config files (in particular web.config)
    /// </summary>
    static class XmlUtil
    {
        public static string ReadSetting(string xmlFilePath, string key)
        {
            var doc = XDocument.Load(xmlFilePath);
            var node = doc.XPathSelectElements(String.Format("/configuration/appSettings/add[@key='{0}']", key)).First();
            return node.Attribute("value").Value;
        }
    }
}
