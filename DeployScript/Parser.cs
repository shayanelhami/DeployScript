namespace DeployScript
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class Parser
    {
        public static readonly Regex KeyValueDefinination = new Regex(@"^\s*(?<key>[^=]+)=\s*(?<value>.*)", RegexOptions.Compiled);
    }
}
