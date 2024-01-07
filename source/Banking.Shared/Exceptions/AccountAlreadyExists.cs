

namespace Banking.Shared.Exceptions
{
    public class AccountAlreadyExists : BadRequestException
    {
        public AccountAlreadyExists(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}