
namespace Banking.Shared.Exceptions
{
    public class UserNotFound : BadRequestException
    {
        public UserNotFound(string errorMessage) : base(errorMessage)
        {
            
        }


    }
}