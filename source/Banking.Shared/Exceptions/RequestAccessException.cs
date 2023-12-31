

namespace Banking.Shared.Exceptions
{
    public sealed class RequestAccessException : BadRequestException
    {
        public RequestAccessException(string message) : base(message)
        {
            
        }
    }
}