

namespace Banking.Shared.Exceptions
{
    public class UserNotFound : BadRequestException
    {
        public UserNotFound(int id) : base($"User with id: {id} doesn't exist in the database")
        {
            
        }

        public UserNotFound(string userName) : base($"User with name: {userName} does not exist in the database")
        {
            
        }
    }
}