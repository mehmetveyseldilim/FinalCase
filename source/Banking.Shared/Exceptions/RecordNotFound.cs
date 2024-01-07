

namespace Banking.Shared.Exceptions
{
    public class RecordNotFound : BadRequestException
    {
        public RecordNotFound(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}