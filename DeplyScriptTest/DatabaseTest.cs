using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeployScript;

namespace DeplyScriptTest
{
    [TestClass]
    public class DatabaseTest
    {
        [TestMethod]
        public void Equal_filter()
        {
            Prepare.Database();
            var variables = new Variables();
            variables.Set(Variables.CONNECTION_STRING, DatabaseUtil.ConnectionString);
            var runner = new Runner(variables);

            runner.ExecScript("DummyFileName", "[database]\nCustomers[name='John Smith'].Age = 90");

            var johnSmithAge = DatabaseUtil.Execute<int>("SELECT TOP 1 age FROM Customers WHERE name ='John Smith'");

            Assert.AreEqual(90, johnSmithAge);
        }

        [TestMethod]
        public void NotEqual_filter()
        {
            Prepare.Database();
            var variables = new Variables();
            variables.Set(Variables.CONNECTION_STRING, DatabaseUtil.ConnectionString);
            var runner = new Runner(variables);

            runner.ExecScript("DummyFileName", "[database]\nCustomers[id<>'11111111-0000-0000-0000-000000000000'].Age = 0");

            var johnDoeAge = DatabaseUtil.Execute<int>("SELECT TOP 1 age FROM Customers WHERE name ='John Doe'");
            var johnSmithAge = DatabaseUtil.Execute<int>("SELECT TOP 1 age FROM Customers WHERE name ='John Smith'");

            Assert.AreEqual(0, johnDoeAge); // must be changed
            Assert.AreEqual(35, johnSmithAge); // must be intact
        }

        [TestMethod]
        public void Set_to_NULL()
        {
            Prepare.Database();
            var variables = new Variables();
            variables.Set(Variables.CONNECTION_STRING, DatabaseUtil.ConnectionString);
            var runner = new Runner(variables);

            runner.ExecScript("DummyFileName", "[database]\nCustomers[id=='11111111-0000-0000-0000-000000000000'].Age = NULL");

            var johnSmithAge = DatabaseUtil.Execute<int?>("SELECT TOP 1 age FROM Customers WHERE name ='John Smith'");

            Assert.IsNull(johnSmithAge); 
        }

        [TestMethod]
        public void Signless_filter()
        {
            Prepare.Database();
            var variables = new Variables();
            variables.Set(Variables.CONNECTION_STRING, DatabaseUtil.ConnectionString);
            var runner = new Runner(variables);

            runner.ExecScript("DummyFileName", "[database]\nCustomers['John Smith'].Age = 80");

            var johnSmithAge = DatabaseUtil.Execute<int?>("SELECT TOP 1 age FROM Customers WHERE name ='John Smith'");

            Assert.AreEqual(80, johnSmithAge);
        }

        [TestMethod]
        public void External_sql_file()
        {
            Prepare.Database();
            Prepare.Script("setTo90.sql", "UPDATE Customers SET Age=90 WHERE Name='John Smith'");
            var variables = new Variables();
            variables.Set(Variables.CONNECTION_STRING, DatabaseUtil.ConnectionString);
            var runner = new Runner(variables);

            runner.ExecScript("DummyFileName", "[database]\nExec setTo90.sql");

            var johnSmithAge = DatabaseUtil.Execute<int?>("SELECT TOP 1 age FROM Customers WHERE name ='John Smith'");

            Assert.AreEqual(90, johnSmithAge);
        }
    }
}
