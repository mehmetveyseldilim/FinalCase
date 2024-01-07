

using Banking.Shared.DTOs.Request;
using FluentValidation;

namespace Banking.API.Validations
{
    public class CreateDepositDTOValidator : AbstractValidator<CreateDepositDTO>
    {
        public CreateDepositDTOValidator()
        {
            RuleFor(account => account.Amount)
                    .GreaterThanOrEqualTo(1)
                    .WithMessage("Deposit amount must be greater than or equal to 1.");
        }
    }
}