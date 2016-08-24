namespace WinTail.Messages
{
    public class FileError
    {
        public FileError(string fileName, string reason)
        {
            FileName = fileName;
            Reason = reason;
        }

        public string FileName { get; }

        public string Reason { get; }
    }
}
