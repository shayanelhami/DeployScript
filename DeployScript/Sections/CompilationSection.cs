namespace DeployScript.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [SectionHeader("compilation")]
    public class CompilationSection : Section
    {
        public override void Execute(string line)
        {
            var match = Parser.KeyValueDefinination.Match(line);
            if (!match.Success)
                throw new ScriptException(Runner.ScriptName, Runner.CurrentLine, "Unknown compilation format:" + line);

            var key = match.Groups["key"].Value.Trim();
            var value = match.Groups["value"].Value.Trim();

            var isValidCompilationAttribute = typeof(System.Web.Configuration.CompilationSection).ContainsConfigurationProperty(key);
            if (!isValidCompilationAttribute)
                throw new ScriptException(Runner.ScriptName, Runner.CurrentLine, "Invalid compilation section attribute:" + key);

            if (Runner.DemoMode) return;
            
            WebConfigManager.UpdateNodeAttribute(
                Runner.Variables,
                nodePath: "/configuration/system.web/compilation",
                nodeDisplayName: "compilation",
                attributeName: key,
                value: value,
                quietMode: Runner.QuietMode);
        }
    }
}
