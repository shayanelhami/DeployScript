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

        public static void UpdateSetting(
            Variables variables,
            string key,
            string value,
            bool quietMode = false)
        {
            UpdateNodeAttribute(
                variables,
                nodePath: String.Format("/configuration/appSettings/add[@key='{0}']", key),
                // Setting nodeDisplayName in this way allows 
                // an error message like 'Cannot find setting Email.Ssl'
                // or info like 'Set setting Email.Ssl value to true'
                nodeDisplayName: "setting " + key,
                attributeName: "value",
                value: value,
                quietMode: quietMode);
        }

        /// <summary>
        /// Updates value of an attribute in the given node (by xpath) or creates the attribute
        /// </summary>
        public static void UpdateNodeAttribute(
            Variables variables,
            string nodePath,
            string nodeDisplayName,
            string attributeName,
            string value,
            bool quietMode = false)
        {
            var doc = XDocument.Load(variables.Get(Variables.PATH_WEBCONFIG));
            var nodes = doc.XPathSelectElements(nodePath);
            if (nodes.Count() == 0)
                throw new Exception(String.Format("Cannot find {0}", nodeDisplayName));

            foreach (var node in nodes)
            {
                if (!quietMode) Console.WriteLine(" Set {0} {1} to {2}", nodeDisplayName, attributeName, value);
                node.SetAttributeValue(attributeName, value);
            }

            doc.Save(variables.Get(Variables.PATH_WEBCONFIG));
        }
    }
}
