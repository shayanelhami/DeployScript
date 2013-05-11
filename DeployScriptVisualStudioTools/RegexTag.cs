namespace DeployScriptVisualStudioTools
{
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.Text.Tagging;

    class RegexTag
    {
        public Regex Pattern { get; set; }
        public ClassificationTag Tag { get; set; }

        /// <summary>
        /// Does not allow other RegexTags to run
        /// </summary>
        public bool IsPredominant { get; set; }

        public RegexTag(Regex pattern, ClassificationTag tag, bool isPredominant = false)
        {
            this.Pattern = pattern;
            this.Tag = tag;
            this.IsPredominant = isPredominant;
        }

        public RegexTag(string pattern, ClassificationTag tag, bool isPredominant = false)
            : this(new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled), tag, isPredominant)
        {
        }
    }
}
