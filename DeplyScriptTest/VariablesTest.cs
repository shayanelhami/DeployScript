using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeployScript;
using System.IO;

namespace DeplyScriptTest
{
    [TestClass]
    public class VariablesTest
    {
        [TestMethod]
        public void PATH_WEB_refers_to_website_folder()
        {
            var webConfig = Prepare.WebConfig();
            var variables = new Variables();

            var actualPath = variables.ReplaceVariables("%path_web%");
            var expectedPath = Path.GetDirectoryName(webConfig);

            Assert.AreEqual(expectedPath, actualPath);
        }

        [TestMethod]
        public void PATH_WEBCONFIG_refers_to_webconfig_file()
        {
            var webConfig = Prepare.WebConfig();
            var variables = new Variables();

            var actualFileName = variables.ReplaceVariables("%path_webconfig%");

            Assert.AreEqual(webConfig, actualFileName);
        }

        [TestMethod]
        public void CONNECTION_STRING_NAME_is_AppDatabase_by_default()
        {
            Prepare.WebConfig();
            var variables = new Variables();

            var actualConnectionStringName = variables.ReplaceVariables("%connection_string_name%");

            Assert.AreEqual("AppDatabase", actualConnectionStringName);
        }

        [TestMethod]
        public void CONNECTION_STRING_is_AppDatabase_value_in_webconfig()
        {
            Prepare.WebConfig();
            var variables = new Variables();

            var actualConnectionStringName = variables.ReplaceVariables("%connection_string%");

            Assert.AreEqual(@"Server=.\SQLEXPRESS; uid=App; password=VERY_SECURE_PASSWORD; MultipleActiveResultSets=True; Database=xyz;", actualConnectionStringName);
        }

        [TestMethod]
        public void DB_is_database_value_extracted_from_connection_string()
        {
            Prepare.WebConfig();
            var variables = new Variables();

            var actualDb = variables.ReplaceVariables("%db%");

            Assert.AreEqual(@"xyz", actualDb);
        }

        [TestMethod]
        public void SERVER_refers_to_machine_name()
        {
            var machineName = Prepare.Machine();
            var variables = new Variables();

            var actualMachine = variables.ReplaceVariables("%server%");

            Assert.AreEqual(machineName, actualMachine);
        }

        [TestMethod]
        public void Overrid_system_variables()
        {
            Prepare.WebConfig();

            var variables = new Variables();
            variables.Set("connection_string_name", "WiredContactDatabase");

            var actualDatabase = variables.ReplaceVariables("%db%");
            Assert.AreEqual("XYZ_2012_11_19", actualDatabase);
        }

        [TestMethod]
        public void Define_variable_and_read_its_value()
        {
            var variables = new Variables();
            variables.Set("hello", "world!");

            var actualValue = variables.ReplaceVariables("%hello%");
            Assert.AreEqual("world!", actualValue);
        }

    }
}
