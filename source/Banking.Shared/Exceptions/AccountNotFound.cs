
namespace Banking.Shared.Exceptions
{
    public class AccountNotFound : BadRequestException
    {
        public AccountNotFound(int id) : base($"Account with id {id} does not exist in the database")
        {
            
        }
    }
}