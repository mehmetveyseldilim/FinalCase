
namespace Banking.Shared.Exceptions
{
    public class InsufficientFundsException : BadRequestException
    {
        public InsufficientFundsException(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}