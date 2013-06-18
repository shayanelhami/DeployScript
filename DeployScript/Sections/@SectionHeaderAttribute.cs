namespace DeployScript.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class SectionHeaderAttribute : Attribute
    {
        public string Header { get; private set; }

        public SectionHeaderAttribute(string header)
        {
            this.Header = header;
        }
    }

}
