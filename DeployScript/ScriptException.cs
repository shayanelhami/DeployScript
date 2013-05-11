namespace DeployScript
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Serializable]
    public class ScriptException : ApplicationException
    {
        public int LineNumber { get; set; }
        public string ScriptName { get; set; }

        public ScriptException() { }
        public ScriptException(string scriptName, int lineNumber, string message)
            : base(message)
        {
            LineNumber = lineNumber;
            ScriptName = scriptName;
        }

        protected ScriptException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

}
