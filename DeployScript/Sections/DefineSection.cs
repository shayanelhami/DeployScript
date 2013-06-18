namespace DeployScript.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [SectionHeader("define")]
    public class DefineSection : Section
    {
        public override void Execute(string line)
        {
            var match = Parser.KeyValueDefinination.Match(line);
            if (!match.Success)
                throw new ScriptException(Runner.ScriptName, Runner.CurrentLine, "Unknown variable definiation format:" + line);

            var key = match.Groups["key"].Value.Trim();
            var value = match.Groups["value"].Value.Trim();

            Runner.Variables.Set(key, value);
        }
    }
}
