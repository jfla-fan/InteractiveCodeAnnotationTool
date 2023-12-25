namespace CodeAnnotationTool.NotionProvider
{
    internal class NotionInfo
    {
        public string Key { get; set; } //< file path or any other identifier
        public int AbsoluteCharOffsetBeginning { get; set; } //< offset from the beginning of the document to the start of text
        public int AbsoluteCharOffsetEnding { get; set; } //< offset from the beginning of the document to the end of text
        public int LinesCount { get; set; } //< how many lines does the selection occupy
        public string Text { get; set; }   //< notion text

        public override string ToString() => $"(Key: {Key}\n" +
                                             $"AbsoluteCharOffsetBeginning: {AbsoluteCharOffsetBeginning}\n" +
                                             $"AbsoluteCharOffsetEnding: {AbsoluteCharOffsetEnding}\n" +
                                             $"LinesCount: {LinesCount}\n" +
                                             $"Text: {Text})\n";
    }
}
