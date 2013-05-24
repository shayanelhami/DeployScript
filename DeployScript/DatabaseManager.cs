namespace DeployScript
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DatabaseManager
    {
        Variables Variables;
        bool QuietMode;
        public delegate void ExecuteSqlHandler(string connectionString, string cmdText);
        public static ExecuteSqlHandler ExecuteSqlOverride = DatabaseManager.ExecuteInSqlServer;

        public DatabaseManager(Variables variables, bool quietMode)
        {
            Variables = variables;
            QuietMode = quietMode;
        }

        public void ExecDatabaseScriptFile(string fileName)
        {
            if (!QuietMode) Console.WriteLine(" Executing " + fileName);
            ExecuteSql(Variables.Get(Variables.CONNECTION_STRING), File.ReadAllText(fileName));
        }

        public void ExecUpdateCommand(string table, string filter, string field, string value)
        {
            if (filter.TrimStart().StartsWith("'")) // it's an operator-less filter
            {
                // filters without operator are considered to be an equal query on Name field
                filter = "Name=" + filter;
            }

            var cmdText = String.Format(" UPDATE {0} SET {1}={2} WHERE {3};",
                Quote(table),
                Quote(field),
                value,
                filter);

            if (!QuietMode) Console.WriteLine(cmdText);
            ExecuteSql(Variables.Get(Variables.CONNECTION_STRING), cmdText);
        }

        private void ExecuteSql(string connectionString, string cmdText)
        {
            ExecuteSqlOverride(connectionString, cmdText);
        }

        /// <summary>
        /// This method provides SQL Server override for DatabaseManager.ExecuteSql. Other components (like test project) can override it.
        /// </summary>
        private static void ExecuteInSqlServer(string connectionString, string cmdText)
        {
            using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new System.Data.SqlClient.SqlCommand(cmdText, conn);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Puts a SQL table or field name into a bracket and takes care of escaping brackets that might be already in the name
        /// </summary>
        private static string Quote(string name)
        {
            return "[" + name.Replace("]", "]]") + "]";
        }
    }
}
