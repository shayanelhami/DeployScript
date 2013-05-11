namespace DeployScriptVisualStudioTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Text.Tagging;
    using Microsoft.VisualStudio.Utilities;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text;
    
    [Export(typeof(ITaggerProvider))]
    [ContentType("DeployScript")]
    [TagType(typeof(ClassificationTag))]
    sealed class DeployScriptClassifierProvider : ITaggerProvider
    {
        [Export]
        [Name("DeployScript")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition DeployScriptContentType = null;

        [Export]
        [FileExtension(".deploy")]
        [ContentType("DeployScript")]
        internal static FileExtensionToContentTypeDefinition DeployScriptFileType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationTypeRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService aggregatorFactory = null;
        
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return new DeployScriptClassifier(buffer, ClassificationTypeRegistry) as ITagger<T>;
        }
    }
}
