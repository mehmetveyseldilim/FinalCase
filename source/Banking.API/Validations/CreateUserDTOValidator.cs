using System.Text.RegularExpressions;
using Banking.Shared.DTOs.Request;
using FluentValidation;

namespace Banking.API.Validations
{
    public class CreateUserDTOValidator : AbstractValidator<CreateUserDTO>
    {
        public CreateUserDTOValidator()
        {
            RuleFor(dto => dto.FirstName).NotEmpty().WithMessage("First Name is required");
            RuleFor(dto => dto.LastName).NotEmpty().WithMessage("Last Name is required");
            RuleFor(dto => dto.UserName).NotEmpty().WithMessage("Turkish Id is required");
            RuleFor(dto => dto.Password).NotEmpty().WithMessage("Password is required");
            RuleFor(dto => dto.Email).NotEmpty().EmailAddress().WithMessage("Invalid Email Address");
            RuleFor(dto => dto.PhoneNumber).NotEmpty().WithMessage("Phone Number is required");

            RuleFor(dto => dto.Roles).NotEmpty().WithMessage("At least one role is required");

            RuleFor(dto => dto.UserName)
                .NotEmpty().WithMessage("User Name is required")
                .Must(BeSerialNumber).WithMessage("User Name must be a string consisting of 11 numbers");

             RuleFor(dto => dto.Password)
                .NotEmpty().WithMessage("Password is required")
                .Must(BeValidPassword).WithMessage("Invalid password format");
        }

        private bool BeSerialNumber(string userName)
        {
            // Use regular expression to validate that userName is a string consisting of 11 numbers
            return Regex.IsMatch(userName, @"^\d{11}$");
        }

        private bool BeValidPassword(string password)
        {
            // Custom validation logic based on the Identity configuration
            return Regex.IsMatch(password, @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{10,}$");
        }

    }
}