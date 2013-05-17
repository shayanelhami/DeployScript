using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployScript
{
    class Program
    {
        static Variables Variables;
        static Arguments Arguments;

        static void Main(string[] args)
        {
            try
            {
                Arguments = Arguments.Create(args);
                if (Arguments.PrintHelp)
                {
                    PrintHelp();
                    return;
                }
                                
                Variables = new Variables();
                if (Arguments.PrintVariables)
                {
                    PrintVariables();
                    return;
                }

                if (Arguments.Scripts.Count == 0)
                {
                    // if no command line argument, execute:
                    // _start.deploy
                    // %server%.deploy 
                    // _other.deploy (if above server file not found)
                    // _finish.deploy

                    RunIfExist("_start.deploy");

                    if (!RunIfExist(Variables.Get(Variables.SERVER) + ".deploy"))
                    {
                        RunIfExist("_other.deploy");
                    }

                    RunIfExist("_finish.deploy");

                }
                else
                {
                    foreach (var script in Arguments.Scripts)
                    {
                        if (!RunIfExist(script))
                            throw new Exception("Cannot find script:" + script);
                    }
                }

            }
            catch (ScriptException err)
            {
                Console.Error.WriteLine("{0}:{1} {2}", err.ScriptName, err.LineNumber, err.Message);
            }
            catch (Exception err)
            {
                Console.Error.WriteLine(Arguments.DebugMode ? err.ToString() : err.Message);
            }

            if (!Arguments.Quiet)
            {
                Console.WriteLine("\nDone. Press any key.");
                Console.ReadKey();
            }
        }

        private static void PrintVariables()
        {
            foreach (var key in Variables.GetSystemVariableNames())
            {
                Console.WriteLine("{0}:\t{1}", key, Variables.Get(key));
            }
        }

        private static void PrintHelp()
        {
            Console.WriteLine(@"
Usage:
deploy_script [-q] [-h] [-v] [script files]

    -q  Quiet mode: Prints no message (unless error) and does not wait at the end for a key press
    -v  Prints system variables and their values
    -d  Debug mode: Prints full stack error messages instead of only a message
    -h  Diplays this help message

    script files:
    One or more .deploy files. If no .deploy file is specified system searches for these files in the current path and runs them:

    _start.deploy
    %server%.deploy (%server% is the value of server variable, see -v option and Server name section below)
    _other.deploy   (if no %server%.deploy file found this one will be executed)
    _finish.deply

    Server name:
    Server name comes from one of these sources:
        Content of C:\servername.txt
        Envirounment variable MACHINE_ALIAS
        or real machine name which is the value of COMPUTERNAME in envirounment variables
");
        }

        /// <summary>
        /// Executes a script if it exists in the current path
        /// </summary>
        /// <param name="script">script name</param>
        /// <returns>false if not found, true if found and executed</returns>
        private static bool RunIfExist(string script)
        {
            if (!File.Exists(script))
            {
                if (!Arguments.Quiet) Console.WriteLine("Not found:" + script);
                return false;
            }

            if (!Arguments.Quiet) Console.WriteLine("Running " + script);
            new Runner(Variables).ExecScript(script);
            return true;
        }        
    }
}
