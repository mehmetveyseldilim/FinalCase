
using Banking.Shared.DTOs.Request;
using FluentValidation;

namespace Banking.API.Validations
{
    public class CreateAccountDTOValidator : AbstractValidator<CreateAccountDTO>
    {
        public CreateAccountDTOValidator()
        {
                RuleFor(account => account.Balance)
                    .GreaterThanOrEqualTo(100)
                    .WithMessage("Balance must be greater than or equal to 100.");

        }
    }
}