namespace WinTail.Messages
{
    public class ValidationError : InputError
    {
        public ValidationError(string reason) : base(reason)
        {
        }
    }
}