using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeAnnotationTool
{
    internal class NotionClassifier : IClassifier
    {
        private IClassificationType _classificationType;
        private ITagAggregator<NotionTag> _tagger;

        internal NotionClassifier(ITagAggregator<NotionTag> tagger, IClassificationType todoType)
        {
            _tagger = tagger;
            _classificationType = todoType;
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            IList<ClassificationSpan> classifiedSpans = new List<ClassificationSpan>();
            var tags = _tagger.GetTags(span);

            foreach (IMappingTagSpan<NotionTag> tagSpan in tags)
            {
                SnapshotSpan todoSpan = tagSpan.Span.GetSpans(span.Snapshot).First();
                classifiedSpans.Add(new ClassificationSpan(todoSpan, _classificationType));
            }

            return classifiedSpans;
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}
