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

        static void Main(string[] args)
        {
            try
            {
                Variables = new Variables();
                if (args.Length == 0)
                {
                    // if no command line argument, execute:
                    // _start.deploy
                    // %server%.deploy 
                    // others.deploy (if above not found)
                    // _finish.deploy
                } else
                {
                    // TODO: check for current directory

                    // if there is a file on the command line argument, runs that and only that
                    var fileName = args[0];
                    if (!File.Exists(fileName))
                        throw new Exception("Cannot find script file:" + fileName);

                    new Runner(Variables).ExecScript(args[0]);
                }

            } catch (ScriptException err)
            {
                Console.Error.WriteLine("{0}:{1} {2}", err.ScriptName, err.LineNumber, err.Message);
            } catch (Exception err)
            {
                Console.Error.WriteLine(err.Message);
            }

            Console.WriteLine("\nDone. Press any key.");
            Console.ReadKey();
        }
    }
}
