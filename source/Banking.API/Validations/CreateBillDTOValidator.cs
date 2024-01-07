using System.Data;
using Banking.Shared.DTOs.Request;
using FluentValidation;

namespace Banking.API.Validations
{
    public class CreateBillDTOValidator : AbstractValidator<CreateBillDTO>
    {
        public CreateBillDTOValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.LastPayTime).GreaterThan(DateTime.UtcNow.AddMonths(-2));
        }
    }
}