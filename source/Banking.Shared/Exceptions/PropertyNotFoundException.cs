

namespace Banking.Shared.Exceptions
{
    public class PropertyNotFoundException : BadRequestException
    {
        public PropertyNotFoundException(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}