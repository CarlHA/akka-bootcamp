namespace WinTail.Messages
{
    public class InitialRead
    {
        public InitialRead(string fileName, string text)
        {
            FileName = fileName;
            Text = text;
        }

        public string FileName { get; }

        public string Text { get; }
    }
}
