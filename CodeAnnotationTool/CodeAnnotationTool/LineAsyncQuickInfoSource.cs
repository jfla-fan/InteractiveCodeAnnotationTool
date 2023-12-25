using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeAnnotationTool.NotionProvider;
using Microsoft.VisualStudio.Shell;

namespace CodeAnnotationTool
{
    internal sealed class LineAsyncQuickInfoSource : IAsyncQuickInfoSource
    {
        private static readonly ImageId _icon = KnownMonikers.AbstractCube.ToImageId();
        private const int _cursorDeltaCharOffset = 0;

        private ITextBuffer _textBuffer;

        public LineAsyncQuickInfoSource(ITextBuffer textBuffer)
        {
            _textBuffer = textBuffer;
        }

        // This is called on a background thread.
        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            var triggerPoint = session.GetTriggerPoint(_textBuffer.CurrentSnapshot);

            if (triggerPoint != null)
            {
                var snapshot = triggerPoint.Value.Snapshot;
                var position = triggerPoint.Value.Position;
                var line = triggerPoint.Value.GetContainingLine();
                var lineNumber = triggerPoint.Value.GetContainingLine().LineNumber;
                var lineSpan = _textBuffer.CurrentSnapshot.CreateTrackingSpan(line.Extent, SpanTrackingMode.EdgeInclusive);

                Debug.WriteLine($"Line number - {lineNumber}");
                Debug.WriteLine($"Line text: {line.GetText()}");
                
                Debug.WriteLine($"Line start, end: {line.Start.Position}, {line.End.Position}");
                
                Debug.WriteLine($"Cursor position: {position}");
                if (position < 0 || position > snapshot.Length)
                {
                    Debug.WriteLine("Position is out of range.");
                }
                else
                {
                    Debug.WriteLine($"Symbols pointed at: {snapshot[position]}");
                }

                var document = _textBuffer.Properties.GetProperty<ITextDocument>(typeof(ITextDocument));
                if (document == null)
                {
                    Debug.WriteLine("Document is null.");
                }
                else
                {
                    Debug.WriteLine($"Document file path: {document.FilePath}");
                }

                int cursorPosition = position - _cursorDeltaCharOffset;
                string showText = null;

                var notionProvider = (INotionProvider) Package.GetGlobalService(typeof(INotionProvider));
                if (notionProvider != null)
                {
                    Debug.WriteLine("Got notion provider in line async quick info source.");
                    if (document != null && document.FilePath != null)
                    {
                        foreach (var notion in notionProvider.FindNotions(document.FilePath))
                        {
                            Debug.WriteLine($"GetQuickInfoItemAsync: {notion}");

                            if (cursorPosition >= notion.AbsoluteCharOffsetBeginning &&
                                cursorPosition <= notion.AbsoluteCharOffsetEnding)
                            {
                                showText = notion.Text;
                                break;
                            }
                        }
                    }
                    
                }
                else
                {
                    Debug.WriteLine("Notion provider is null.");
                }

                if (showText != null)
                {
                    var element = new ContainerElement(
                        ContainerElementStyle.Wrapped,
                        new ImageElement(_icon),
                        new ClassifiedTextElement(
                            new ClassifiedTextRun(PredefinedClassificationTypeNames.Text, showText))
                    );

                    return Task.FromResult(new QuickInfoItem(lineSpan, element));
                }

                return Task.FromResult<QuickInfoItem>(null);
            }

            return Task.FromResult<QuickInfoItem>(null);
        }

        public void Dispose()
        {
            // This provider does not perform any cleanup.
        }
    }
}
