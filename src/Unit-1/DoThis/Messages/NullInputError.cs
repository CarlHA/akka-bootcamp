namespace WinTail.Messages
{
    public class NullInputError : InputError
    {
        public NullInputError(string reason) : base(reason)
        {
        }
    }
}