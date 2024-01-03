
namespace Banking.Shared.Exceptions
{
    public class OperationLimitExceededException : BadRequestException
    {
        public OperationLimitExceededException(string errorMessage) : base(errorMessage)
        {

        }
    }
}