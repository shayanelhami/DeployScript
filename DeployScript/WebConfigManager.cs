namespace DeployScript
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.XPath;

    class WebConfigManager
    {
        public static string GetConnectionString(Variables variables, string name)
        {
            var doc = XDocument.Load(variables.Get(Variables.PATH_WEBCONFIG));
            var node = doc.XPathSelectElement(String.Format("/configuration/connectionStrings/add[@name='{0}']", name));
            if (node == null)
                throw new Exception("Cannot find a connection string with name " + name);

            var attribute = node.Attribute("connectionString");
            if (attribute == null)
                throw new Exception("No connectionString attribute found in connection string node");

            return attribute.Value;
        }

        public static IEnumerable<string> PossibleSettingKeys(Variables variables)
        {
            var doc = XDocument.Load(variables.Get(Variables.PATH_WEBCONFIG));
            return doc.XPathSelectElements("/configuration/appSettings/add").Select(n =>
                n.Attribute("key").Value);
        }

        public static void UpdateSetting(Variables variables, string key, string value)
        {
            var doc = XDocument.Load(variables.Get(Variables.PATH_WEBCONFIG));
            var nodes = doc.XPathSelectElements(String.Format("/configuration/appSettings/add[@key='{0}']", key));
            if (nodes.Count() == 0)
                throw new Exception("Cannot find any settings with key " + key);

            foreach (var node in nodes)
            {
                node.Attribute("value").Value = value;
            }

            doc.Save(variables.Get(Variables.PATH_WEBCONFIG));
        }
    }
}
