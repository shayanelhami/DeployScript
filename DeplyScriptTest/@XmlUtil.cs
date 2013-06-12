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
        /// <summary>
        /// Reads a setting from appSettings section which has the given key
        /// </summary>
        public static string ReadSetting(string xmlFilePath, string key)
        {
            return ReadAttribute(
                xmlFilePath,
                nodepath: String.Format("/configuration/appSettings/add[@key='{0}']", key),
                attributeName: "value");
        }

        /// <summary>
        /// Reads the value of 'from' field in smtp settings
        /// </summary>
        public static string ReadSmtpField(string xmlFilePath, string fieldName)
        {
            return ReadAttribute(
                xmlFilePath,
                nodepath: "/configuration/system.net/mailSettings/smtp",
                attributeName: fieldName);
        }

        /// <summary>
        /// Reads the value of given field in smtp settings, network node
        /// </summary>
        public static string ReadSmtpNetworkField(string xmlFilePath, string fieldName)
        {
            return ReadAttribute(
                xmlFilePath,
                nodepath: "/configuration/system.net/mailSettings/smtp/network",
                attributeName: fieldName);
        }

        /// <summary>
        /// Returns value of the given attribute in the given node (xpath) or null if node or attribute does no exist
        /// </summary>
        private static string ReadAttribute(string xmlFilePath, string nodepath, string attributeName)
        {
            var doc = XDocument.Load(xmlFilePath);
            var node = doc.XPathSelectElements(nodepath).FirstOrDefault();

            if (node == null)
                return null;

            var att = node.Attribute(attributeName);

            if (att == null)
                return null;

            return att.Value;
        }
    }
}
