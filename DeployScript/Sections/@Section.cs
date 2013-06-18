namespace DeployScript.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class Section
    {
        public IRunner Runner { get; set; }

        public abstract void Execute(string line);
    }
}
