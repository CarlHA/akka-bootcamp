namespace WinTail.Messages
{
    public class StopTail
    {
        public string FilePath { get; }

        public StopTail(string filePath)
        {
            FilePath = filePath;
        }
    }
}
