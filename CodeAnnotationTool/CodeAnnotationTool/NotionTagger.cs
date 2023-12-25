using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using CodeAnnotationTool.NotionProvider;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Threading;

namespace CodeAnnotationTool
{
    internal class NotionTagger : ITagger<NotionTag>
    {
        private INotionProvider _notionProvider;
        private DTE _dte;

        public NotionTagger(INotionProvider notionProvider, DTE dte)
        {
            _notionProvider = notionProvider;
            _dte = dte;
        }

        public IEnumerable<ITagSpan<NotionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_notionProvider is null || _dte is null || _dte.ActiveDocument is null || _dte.ActiveDocument.FullName is null)
                yield return null;

            string documentPath = _dte.ActiveDocument.FullName;

            IList<NotionInfo> notions = _notionProvider.FindNotions(documentPath).ToList();

            foreach (SnapshotSpan curSpan in spans)
            {
                Debug.WriteLine($"start, end, length: {curSpan.Start.Position}, {curSpan.End.Position}, {curSpan.Length}");
                Debug.WriteLine($"text: {curSpan.GetText()}");

                foreach (var notion in notions)
                {
                    Span notionSpan = new Span(notion.AbsoluteCharOffsetBeginning, notion.AbsoluteCharOffsetEnding - notion.AbsoluteCharOffsetBeginning);
                    SnapshotSpan? intersectionSpan = curSpan.Intersection(notionSpan);

                    Debug.WriteLine($"Tried to find intersection with: {notionSpan.Start}, {notionSpan.End}");

                    if (intersectionSpan != null)
                    {
                        Debug.WriteLine($"Got intersection: {intersectionSpan.Value.Start.Position}, {intersectionSpan.Value.End.Position}");
                        yield return new TagSpan<NotionTag>(intersectionSpan.Value, new NotionTag());
                    }
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }

    internal class NotionTag : TextMarkerTag
    {
        public NotionTag() : base("NotionTag/NotionTagFormatDefinition")
        {
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "notion")]
    [Name("Notion highlighting")]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    internal class NotionTagFormatDefinition : ClassificationFormatDefinition
    {
        public NotionTagFormatDefinition()
        {
            TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
}
