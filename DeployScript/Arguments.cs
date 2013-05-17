using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployScript
{
    /// <summary>
    /// A class to parse command line arguments
    /// </summary>
    class Arguments
    {
        /// <summary>
        /// Accepts the same args string array which system passes to Main() and returns a typed version of it.
        /// </summary>
        public static Arguments Create(string[] args)
        {
            var result = new Arguments();
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case "-v":
                        result.PrintVariables = true;
                        break;
                    case "-d":
                        result.DebugMode = true;
                        break;
                    case "-?":
                    case "-h":
                    case "--help":
                        result.PrintHelp = true;
                        break;
                    case "-q":
                        result.Quiet = true;
                        break;
                    default:
                        result.Scripts.Add(arg);
                        break;
                }
            }

            return result;
        }

        private Arguments()
        {
            Scripts = new List<string>();
        }

        public bool PrintVariables { get; private set; }
        public bool Quiet { get; private set; }
        public bool PrintHelp { get; private set; }
        public bool DebugMode { get; private set; }
        public List<string> Scripts { get; private set; }
    }
}
