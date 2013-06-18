namespace DeployScript.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Configuration;
    using System.Text;
    using System.Threading.Tasks;

    [SectionHeader("smtp")]
    public class SmtpSection : Section
    {
        public override void Execute(string line)
        {
            var match = Parser.KeyValueDefinination.Match(line);
            if (!match.Success)
                throw new ScriptException(Runner.ScriptName, Runner.CurrentLine, "Unknown smtp format:" + line);

            var key = match.Groups["key"].Value.Trim();
            var value = match.Groups["value"].Value.Trim();

            // decide whether item belongs to Smtp node or Network node
            var isInNetworkSection = typeof(SmtpNetworkElement).ContainsConfigurationProperty(key);

            if (Runner.DemoMode) return;
            
            WebConfigManager.UpdateNodeAttribute(
                Runner.Variables,
                nodePath: isInNetworkSection ?
                    "/configuration/system.net/mailSettings/smtp/network" :
                    "/configuration/system.net/mailSettings/smtp",
                nodeDisplayName: isInNetworkSection ? "smtp/network" : "smtp",
                attributeName: key,
                value: value,
                quietMode: Runner.QuietMode);
        }
    }
}
