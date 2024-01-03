
namespace Banking.Shared.Exceptions
{
    public class DailyLimitExceededException : BadRequestException
    {
        public DailyLimitExceededException(string errorMessage) : base(errorMessage)
        {
            
        }
    }
}