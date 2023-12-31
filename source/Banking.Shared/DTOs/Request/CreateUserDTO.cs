namespace Banking.Shared.DTOs.Request
{
    public class CreateUserDTO : BaseValidationModel<CreateUserDTO>
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public required string UserName { get; init; }
        public required string Password { get; init; }
        public required string Email { get; init; }
        public required string PhoneNumber { get; init; }
        public required ICollection<string> Roles { get; init; }
    }
}