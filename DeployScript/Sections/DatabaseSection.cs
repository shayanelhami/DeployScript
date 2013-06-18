namespace DeployScript.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    [SectionHeader("database")]
    class DatabaseSection : Section
    {
        Regex DatabaseCommandDefinination = new Regex(@"^\s*(?<table>[^[]+)\[(?<filter>[^\]]+)\]\.(?<field>[^=]+)\s*=\s*(?<value>.+)", RegexOptions.Compiled);
        Regex ExecCommandDefinination = new Regex(@"^\s*Exec\s*(?<filename>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public override void Execute(string line)
        {
            var databaseManager = new DatabaseManager(Runner.Variables, Runner.QuietMode);

            var match = DatabaseCommandDefinination.Match(line);
            if (match.Success)
            {
                if (Runner.DemoMode) return;

                databaseManager.ExecUpdateCommand(
                    match.Groups["table"].Value,
                    match.Groups["filter"].Value,
                    match.Groups["field"].Value,
                    match.Groups["value"].Value);
                return;
            }

            match = ExecCommandDefinination.Match(line);
            if (match.Success)
            {
                if (Runner.DemoMode) return;

                databaseManager.ExecDatabaseScriptFile(match.Groups["filename"].Value);
                return;
            }

            throw new ScriptException(Runner.ScriptName, Runner.CurrentLine, "Unknown database format:" + line);
        }
    }
}
