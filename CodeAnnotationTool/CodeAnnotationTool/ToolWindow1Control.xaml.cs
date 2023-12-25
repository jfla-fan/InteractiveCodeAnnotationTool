using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using CodeAnnotationTool.NotionProvider;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace CodeAnnotationTool
{
    /// <summary>
    /// Interaction logic for ToolWindow1Control.
    /// </summary>
    public partial class ToolWindow1Control : UserControl
    {
        private const int textSelectionCharOffsetDelta = 6;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindow1Control"/> class.
        /// </summary>
        public ToolWindow1Control()
        {
            this.InitializeComponent();
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Got ok button click");
            Debug.WriteLine($"Got text: {this.TextInput.Text}");

            ThreadHelper.ThrowIfNotOnUIThread();

            DTE dte = (DTE) Package.GetGlobalService(typeof(DTE));
            if (dte.ActiveDocument?.Selection != null)
            {
                var textSelection = (TextSelection) dte.ActiveDocument.Selection;
                var bottomPoint = textSelection.BottomPoint;
                var topPoint = textSelection.TopPoint;

                Debug.WriteLine($"Bottom point line, offset, absolute char offset, column:" +
                                $"{bottomPoint.Line}, {bottomPoint.LineCharOffset}, " +
                                $"{bottomPoint.AbsoluteCharOffset}, {bottomPoint.DisplayColumn},");

                Debug.WriteLine($"Top point line, offset, absolute char offset, column:" +
                                $"{topPoint.Line}, {topPoint.LineCharOffset}, " +
                                $"{topPoint.AbsoluteCharOffset}, {topPoint.DisplayColumn},");

                Debug.WriteLine($"Selected text: {textSelection.Text}");


                INotionProvider notionProvider = (INotionProvider) Package.GetGlobalService(typeof(INotionProvider));
                if (notionProvider != null)
                {
                    NotionInfo notion = new NotionInfo()
                    {
                        AbsoluteCharOffsetBeginning = topPoint.AbsoluteCharOffset + textSelectionCharOffsetDelta,
                        AbsoluteCharOffsetEnding = bottomPoint.AbsoluteCharOffset + textSelectionCharOffsetDelta,
                        Key = dte.ActiveDocument.FullName,
                        LinesCount = textSelection.BottomLine - textSelection.TopLine + 1,
                        Text = this.TextInput.Text,
                    };

                    notionProvider.SaveNotion(notion);
                }
            }
            else
            {
                Debug.WriteLine("Failed to obtain active document or selection.");
            }
        }
    }
}