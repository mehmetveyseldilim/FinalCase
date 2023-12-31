

namespace Banking.Shared.Exceptions
{
    public class UserNotFound : BadRequestException
    {
        public UserNotFound(string id) : base($"User with name: {id} doesn't exist in the database")
        {
            
        }

        public UserNotFound(int id) : base($"User with name: {id} doesn't exist in the database")
        {
            
        }
    }
}