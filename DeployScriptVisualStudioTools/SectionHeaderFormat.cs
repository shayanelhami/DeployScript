namespace DeployScriptVisualStudioTools
{
    using System.ComponentModel.Composition;
    using System.Windows.Media;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "SectionHeader")]
    [Name("SectionHeader")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    sealed class SectionHeaderFormat : ClassificationFormatDefinition
    {
        public SectionHeaderFormat()
        {
            this.DisplayName = "Section header";
            this.ForegroundColor = Colors.Orange;
        }

        static class SpecialElementClassificationDefinition
        {
            [Export(typeof(ClassificationTypeDefinition))]
            [Name("SectionHeader")]
            internal static ClassificationTypeDefinition SectionHeaderType = null;
        }
    }
}

