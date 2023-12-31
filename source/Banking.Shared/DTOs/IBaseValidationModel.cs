using FluentValidation.Results;

namespace Banking.Shared.DTOs
{
    public interface IBaseValidationModel
    {
        public ValidationResult? Validate(object validator, IBaseValidationModel modelObj);
    }
}