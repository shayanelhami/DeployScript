namespace DeployScript.Sections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    class SectionFactory
    {
        /// <summary>
        /// A cache to map section header to an instance of section type
        /// </summary>
        static Dictionary<string, Section> SectionMap;

        /// <summary>
        /// Finds a section with the heading similar to the one in line.
        /// Returns null if it cannot find any proper section type
        /// </summary>
        public static Section FindSection(IRunner runner, string line)
        {
            if (SectionMap == null)
            {
                FillSectionMap(runner);
            }

            if (SectionMap.ContainsKey(line))
                return SectionMap[line] as Section;

            return null;
        }

        private static void FillSectionMap(IRunner runner)
        {
            SectionMap = new Dictionary<string, Section>();

            // We may later read the current directory for dlls to load
            // 3rd party section handler modules but right now
            // there is not such a requirement

            // get types inherited from Section
            var types =
                typeof(Section).Assembly.
                GetTypes().
                Where(t => typeof(Section).IsAssignableFrom(t)).
                ToArray();

            foreach (var type in types)
            {
                var heading = type.GetCustomAttributes(
                        typeof(SectionHeaderAttribute),
                        inherit: false).
                    OfType<SectionHeaderAttribute>().
                    FirstOrDefault();

                // only accept Sections types which have SectionHeader attribute
                if (heading != null)
                {
                    var instance = Activator.CreateInstance(type) as Section;
                    instance.Runner = runner;
                    // cache the concrete section handler
                    SectionMap.Add("[" + heading.Header.ToLower() + "]", instance);
                }
            }
        }
    }
}
