using System;

namespace CodeAnnotationTool.NotionProvider
{
    internal class NotionProviderException : Exception
    {
        public NotionProviderException() : base() { }
        public NotionProviderException(string message) : base(message) { }
        public NotionProviderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
