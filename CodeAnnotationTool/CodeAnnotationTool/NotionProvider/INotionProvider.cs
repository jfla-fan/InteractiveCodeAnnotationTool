using System.Collections.Generic;
using System;

namespace CodeAnnotationTool.NotionProvider
{
    /// <summary>
    /// Provides access to notion storage.
    /// </summary>
    internal interface INotionProvider : IDisposable
    {
        IEnumerable<NotionInfo> GetNotions(string key);
        IEnumerable<NotionInfo> FindNotions(string key);
        int RemoveNotion(Predicate<NotionInfo> predicate);
        bool SaveNotion(NotionInfo notionInfo);
        bool ExistsNotion(Predicate<NotionInfo> predicate);
        void Refresh();
        void Flush();
    }
}
