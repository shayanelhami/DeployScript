using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace DeplyScriptTest
{
    /// <summary>
    /// Provides a set of utilities to read database
    /// </summary>
    static class DatabaseUtil
    {
        /// <summary>
        /// A folder that database file will be stored there
        /// </summary>
        public static string DatabasePath
        {
            get { return Path.Combine(Environment.CurrentDirectory, "Database"); }
        }

        /// <summary>
        /// Full path to the database file
        /// </summary>
        public static string DatabaseFile
        {
            get { return Path.Combine(DatabasePath, "db.sdf"); }
        }

        /// <summary>
        /// Connection string that can be used to access database file
        /// </summary>
        public static string ConnectionString
        {
            get { return "Data Source=" + DatabaseFile; }
        }

        /// <summary>
        /// Executes a SQL command and returns its value as a string
        /// </summary>
        public static string Execute(string sql)
        {            
            return Execute<string>(sql);
        }

        /// <summary>
        /// Executes a SQL command and returns its value as a given type
        /// </summary>
        public static T Execute<T>(string sql)
        {
            var conn = new SqlCeConnection(DatabaseUtil.ConnectionString);
            conn.Open();
            var cmd = new SqlCeCommand(sql, conn);
            var result = (T)cmd.ExecuteScalar() ;
            conn.Close();

            return result;
        }
    }
}
