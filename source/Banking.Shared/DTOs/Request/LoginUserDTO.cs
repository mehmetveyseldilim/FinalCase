namespace Banking.Shared.DTOs.Request
{
    public class LoginUserDTO : BaseValidationModel<LoginUserDTO>
    {
        public required string UserName { get; init; }
        public required string Password { get; init; }
    }
}