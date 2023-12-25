using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnnotationTool
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("code")]
    internal class NotionClassifierProvider : IClassifierProvider
    {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("notion")]
        internal ClassificationTypeDefinition ToDoClassificationType = null;

        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null;

        [Import]
        internal IBufferTagAggregatorFactoryService _tagAggregatorFactory = null;

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            IClassificationType classificationType = ClassificationRegistry.GetClassificationType("notion");

            var tagAggregator = _tagAggregatorFactory.CreateTagAggregator<NotionTag>(textBuffer);
            return new NotionClassifier(tagAggregator, classificationType);
        }
    }
}
