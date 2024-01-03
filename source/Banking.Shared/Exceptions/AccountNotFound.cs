
namespace Banking.Shared.Exceptions
{
    public class AccountNotFound : BadRequestException
    {
        public AccountNotFound(string errorMessage) : base(errorMessage)
        {
            
        }

    }
}