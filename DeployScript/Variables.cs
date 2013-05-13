using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeployScript
{
    public class Variables
    {
        // keys to system variables
        public const string SERVER = "server";
        public const string DB = "db";
        public const string PATH_WEB = "path_web";
        public const string PATH_WEBCONFIG = "path_webconfig";
        public const string CONNECTION_STRING = "connection_string";
        public const string CONNECTION_STRING_NAME = "connection_string_name";

        // the real storage place of variables
        private Dictionary<string, string> Bank;

        // helps in lazy binding of server variables
        private Dictionary<string, Func<string>> SystemVariableFinders;

        public Variables()
        {
            Bank = new Dictionary<string, string>();
            SystemVariableFinders = new Dictionary<string, Func<string>>();
            SystemVariableFinders.Add(Variables.SERVER, FindServerName);
            SystemVariableFinders.Add(Variables.DB, FindDatabaseName);
            SystemVariableFinders.Add(Variables.PATH_WEB, FindPathWeb);
            SystemVariableFinders.Add(Variables.PATH_WEBCONFIG, FindPathWebConfig);
            SystemVariableFinders.Add(Variables.CONNECTION_STRING, FindConnectionString);
            SystemVariableFinders.Add(Variables.CONNECTION_STRING_NAME, FindConnectionStringName);
        }

        public IEnumerable<string> GetSystemVariableNames()
        {
            return SystemVariableFinders.Keys;
        }

        /// <summary>
        /// Changes the value of a variable in bank or adds it.
        /// In case of assigning an empty or null value to key, un-defines it
        /// </summary>
        public void Set(string key, string value)
        {
            if (!value.HasValue())
            {
                Undefine(key);
            } else
            {
                if (!Bank.ContainsKey(key))
                {
                    Bank.Add(key, value);
                } else
                {
                    Bank[key] = value;
                }
            }
        }

        /// <summary>
        /// Removes the definition and the value of the variable.
        /// In case of system variables it causes them to return to default and system define values
        /// </summary>
        public void Undefine(string key)
        {
            Bank.Remove(key);
        }

        /// <summary>
        /// Uses for late evaluation of system variables.
        /// For example if a deploy script overrides connectionString in [define] section
        /// there will not be any need to evaluate it here (which may cause an error about 
        /// not finding webconfig if we first try set system variables) Also it boosts startup
        /// of the application
        /// </summary>
        public string Get(string key)
        {
            string value = null;
            if (Bank.TryGetValue(key, out value))
                return value;

            if (SystemVariableFinders.ContainsKey(key))
            {
                // we always re-evaluate system variables
                // it is possible for use to change a variable (for example %connection_string_name%)
                // and expect other variables related to it to change accordingly (like %db%)
                // so we don't store the value in Bank
                return SystemVariableFinders[key]();
            }

            throw new Exception("Unknown variable:" + key);
        }

        public Regex WrappedVariable = new Regex(@"(?<!\\)%(?<key>(\\%|[^%])+)%", RegexOptions.Compiled);

        public string ReplaceVariables(string str)
        {
            return WrappedVariable.Replace(str, m =>
            {
                return Get(m.Groups["key"].Value
                    // we accept escape inside a variable name
                    .Replace("\\%", "%"));
            })
                // we also accept escaped % outside of a variable name
                .Replace("\\%", "%");
        }

        private string FindServerName()
        {
            return GetServerNameFromTextFile() ??
                Environment.GetEnvironmentVariable("MACHINE_ALIAS") ??
                Environment.MachineName;
        }

        private string GetServerNameFromTextFile()
        {
            try
            {
                return File.ReadAllText(@"C:\servername.txt").Trim();
            } catch
            {
                return null;
            }
        }

        private string FindPathWeb()
        {
            var pathWeb = Path.Combine(Environment.CurrentDirectory, "Website");
            if (Directory.Exists(pathWeb))
                return pathWeb;

            pathWeb = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..", "Website"));
            if (Directory.Exists(pathWeb))
                return pathWeb;

            throw new Exception("Cannot find path to Website folder");
        }

        private string FindPathWebConfig()
        {
            var webConfig = Path.Combine(Get(Variables.PATH_WEB), "web.config");
            if (!File.Exists(webConfig))
                throw new Exception("Cannot find web.config file in " + webConfig);

            return webConfig;
        }

        private string FindConnectionStringName()
        {
            return "AppDatabase";
        }

        private string FindConnectionString()
        {
            return WebConfigManager.GetConnectionString(this,Get(Variables.CONNECTION_STRING_NAME));
        }

        private string FindDatabaseName()
        {
            var connectionString = Get(Variables.CONNECTION_STRING);
            var parser = new SqlConnectionStringBuilder(connectionString);
            var databaseName = parser.InitialCatalog;
            if (!databaseName.HasValue())
                throw new Exception("Database name does not exist in connection string " + connectionString);

            return databaseName;
        }

    }
}
