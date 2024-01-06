
namespace Banking.Shared.DTOs.Request
{
    public class CreateAccountDTO : BaseValidationModel<CreateAccountDTO>
    {
        public int UserId {get; set;}
        public int Balance {get; set;}
    }
}