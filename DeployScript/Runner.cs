namespace DeployScript
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public enum Section
    {
        None,
        Settings,
        Database,
        Define
    }

    public class Runner
    {
        /// <summary>
        /// To prevent circular references full path to every included script (including the current one)
        /// will be put in this list
        /// </summary>
        List<string> IncludedScripts;
        Variables Variables;
        string ScriptName = null;

        public Section CurrentSection { get; private set; }
        public int CurrentLine { get; private set; }

        /// <summary>
        /// In demo mode system does not change anything but goes over all lines
        /// </summary>
        public bool DemoMode { get; set; }

        /// <summary>
        /// In quiet mode does not print any log on standard output
        /// </summary>
        public bool QuietMode { get; set; }

        /// <summary>
        /// If anything other than null system runs until the given line number and stops there
        /// </summary>
        public int? RunToLine { get; set; }

        public Runner(Variables variables, List<string> includedScripts = null, bool demoMode = false, bool quietMode = false)
        {
            Variables = variables;
            IncludedScripts = includedScripts ?? new List<string>();
            DemoMode = demoMode;
            CurrentSection = Section.None;
            QuietMode = quietMode;
        }

        /// <summary>
        /// Executes a script from content
        /// </summary>
        /// <param name="fileName">Filename to appear in debug (full path)</param>
        /// <param name="content">The content (UTF8)</param>
        public void ExecScript(string fileName, string content)
        {
            ExecScript(fileName, new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(content))));
        }

        /// <summary>
        /// Executes a script from file
        /// </summary>
        public void ExecScript(string scriptPath)
        {
            using (var reader = File.OpenText(scriptPath))
            {
                ExecScript(scriptPath, reader);
            }
        }

        /// <summary>
        /// Executes a script from Stream.
        /// </summary>
        /// <param name="fileName">Filename to appear in debug (full path)</param>
        /// <param name="reader">The stream</param>
        public void ExecScript(string fileName, StreamReader reader)
        {
            ScriptName = Path.GetFileName(fileName);
            IncludedScripts.Add(fileName.ToLower());
            CurrentLine = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                CurrentLine++;

                if (CurrentLine >= RunToLine)
                    return;

                if (line.HasValue() && !line.StartsWith("#"))
                {
                    if (line.Contains('%'))
                    {
                        try
                        {
                            line = Variables.ReplaceVariables(line);
                        }
                        catch (Exception err)
                        {
                            // if variable is not found there is a change to report the line number here
                            throw new ScriptException(ScriptName, CurrentLine, err.Message);
                        }
                    }

                    ExecLine(line);
                }
            }
        }

        /// <summary>
        /// Includes a script. Makes sure there is no circular reference
        /// </summary>
        private void IncludeScript(string scriptPath)
        {
            if (IncludedScripts.Contains(scriptPath.ToLower()))
                throw new ScriptException(ScriptName, CurrentLine, "Including '{0}' will cause circular reference. Cannot proceed");

            if (!File.Exists(scriptPath))
                throw new ScriptException(ScriptName, CurrentLine, "Cannot find script file:" + scriptPath);

            new Runner(Variables, IncludedScripts, DemoMode).ExecScript(scriptPath);
        }

        const string IncludeCommand = "include ";
        const string PrintCommand = "print ";

        private void ExecLine(string input)
        {
            // special commands
            if (ExecuteSecialCommand(input))
                return;

            // sections
            var section = FindSection(input);
            if (section.HasValue)
            {
                CurrentSection = section.Value;
                return;
            }

            ExecuteInSection(input);
        }

        private void ExecuteInSection(string input)
        {
            switch (CurrentSection)
            {
                case Section.Settings:
                    ExceSettingsCommand(input);
                    break;
                case Section.Database:
                    ExecDatabaseCommand(input);
                    break;
                case Section.Define:
                    ExecDefineCommand(input);
                    break;
                default:
                    throw new ScriptException(ScriptName, CurrentLine, "Command outside of all sections:" + input);
            }
        }

        /// <summary>
        /// Executes a special command (like print or include) if found on the line
        /// </summary>
        /// <returns>true if the was a special command</returns>
        private bool ExecuteSecialCommand(string input)
        {
            var line = input.ToLower();
            if (line.StartsWith(IncludeCommand))
            {
                IncludeScript(input.Substring(IncludeCommand.Length));
                return true;
            }

            if (line.StartsWith(PrintCommand))
            {
                Console.WriteLine(input.Substring(PrintCommand.Length));
                return true;
            }

            return false;
        }

        private Section? FindSection(string input)
        {
            var line = input.ToLower();

            if (line == "[define]") return Section.Define;
            if (line == "[settings]") return Section.Settings;
            if (line == "[database]") return Section.Database;

            if (line.StartsWith("["))
                throw new ScriptException(ScriptName, CurrentLine, "Unknown section:" + line);

            return null;
        }

        Regex KeyValueDefinination = new Regex(@"^\s*(?<key>[^=]+)=\s*(?<value>.*)", RegexOptions.Compiled);

        private void ExecDefineCommand(string input)
        {
            var match = KeyValueDefinination.Match(input);
            if (!match.Success)
                throw new ScriptException(ScriptName, CurrentLine, "Unknown variable definiation format:" + input);

            var key = match.Groups["key"].Value.Trim();
            var value = match.Groups["value"].Value.Trim();

            Variables.Set(key, value);
        }

        private void ExceSettingsCommand(string input)
        {
            var match = KeyValueDefinination.Match(input);
            if (!match.Success)
                throw new ScriptException(ScriptName, CurrentLine, "Unknown setting format:" + input);

            var key = match.Groups["key"].Value.Trim();
            var value = match.Groups["value"].Value.Trim();

            if (DemoMode) return;

            WebConfigManager.UpdateSetting(Variables, key, value, QuietMode);
        }

        Regex DatabaseCommandDefinination = new Regex(@"^\s*(?<table>[^[]+)\[(?<filter>[^\]]+)\]\.(?<field>[^=]+)\s*=\s*(?<value>.+)", RegexOptions.Compiled);
        Regex ExecCommandDefinination = new Regex(@"^\s*Exec\s*(?<filename>.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private void ExecDatabaseCommand(string input)
        {
            var databaseManager = new DatabaseManager(Variables, QuietMode);

            var match = DatabaseCommandDefinination.Match(input);
            if (match.Success)
            {
                if (DemoMode) return;

                databaseManager.ExecUpdateCommand(
                    match.Groups["table"].Value,
                    match.Groups["filter"].Value,
                    match.Groups["field"].Value,
                    match.Groups["value"].Value);
                return;
            }

            match = ExecCommandDefinination.Match(input);
            if (match.Success)
            {
                if (DemoMode) return;

                databaseManager.ExecDatabaseScriptFile(match.Groups["filename"].Value);
                return;
            }

            throw new ScriptException(ScriptName, CurrentLine, "Unknown database format:" + input);
        }
       
    }
}
