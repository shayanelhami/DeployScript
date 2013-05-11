namespace DeployScriptVisualStudioTools
{
    using System.ComponentModel.Composition;
    using System.Windows.Media;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Utilities;

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Variable")]
    [Name("Variable")]
    [UserVisible(true)]
    [Order(Before = Priority.Default)]
    sealed class VariableFormat : ClassificationFormatDefinition
    {
        public VariableFormat()
        {
            this.DisplayName = "Varible";
            this.ForegroundColor = Colors.Black;
            this.BackgroundColor= Colors.Azure;
        }

        static class SpecialElementClassificationDefinition
        {
            [Export(typeof(ClassificationTypeDefinition))]
            [Name("Variable")]
            internal static ClassificationTypeDefinition VariableType = null;
        }
    }
}

