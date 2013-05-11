namespace DeployScriptVisualStudioTools
{
    class Region
    {
        public int StartLine { get; set; }
        public int StartOffset { get; set; }
        public int EndLine { get; set; }
        public int EndOffset { get; set; }

        public string Ellipsis { get; set; }
    }
}
