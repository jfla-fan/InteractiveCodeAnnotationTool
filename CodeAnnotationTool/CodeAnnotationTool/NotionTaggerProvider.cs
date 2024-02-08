using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeAnnotationTool.NotionProvider;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace CodeAnnotationTool
{
    [Export(typeof(ITaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(NotionTag))]
    internal class NotionTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            CachedNotionProvider notionProvider = (CachedNotionProvider) Package.GetGlobalService(typeof(CachedNotionProvider));
            DTE dte = (DTE) Package.GetGlobalService(typeof(DTE));
            
            return new NotionTagger(notionProvider, dte) as ITagger<T>;
        }
    }
}
