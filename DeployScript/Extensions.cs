using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployScript
{
    static class Extensions
    {
        public static bool HasValue(this string str)
        {
            return !String.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Determins if the given key is a property of Configuration Section type 
        /// (as defined by ConfigurationPropertyAttribute attribute of properties of the section type)
        /// </summary>
        public static bool ContainsConfigurationProperty(this Type type, string key)
        {
            var properties = type.GetProperties();
            var configurationProperties = properties.SelectMany(p =>
                p.GetCustomAttributes(typeof(ConfigurationPropertyAttribute), inherit: false)).
                Cast<ConfigurationPropertyAttribute>().
                Select(a => a.Name.ToLower());

            return configurationProperties.Contains(key.ToLower());
        }
    }
}
