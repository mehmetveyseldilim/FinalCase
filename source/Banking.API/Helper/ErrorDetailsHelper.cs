using Banking.Shared;
using FluentValidation.Results;

namespace Banking.API.Helper
{
    public static class ErrorDetailsHelper
    {
        public static ErrorDetails CreateErrorDetails(ValidationResult validationResult)
        {
            var errorMessages = validationResult.Errors.Select(x => x.ErrorMessage).ToList();

            return new ErrorDetails()
            {
                StatusCode = 400,
                Message = errorMessages
            };
        }
    }
}