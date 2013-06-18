namespace DeployScript
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IRunner
    {
        /// <summary>
        /// In demo mode system does not change anything but goes over all lines
        /// </summary>
        bool DemoMode { get; }

        /// <summary>
        /// In quiet mode does not print any log on standard output
        /// </summary>
        bool QuietMode { get; }

        string ScriptName { get; }
        int CurrentLine { get; }

        Variables Variables { get; }
    }
}
