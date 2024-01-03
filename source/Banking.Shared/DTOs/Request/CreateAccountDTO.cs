
namespace Banking.Shared.DTOs.Request
{
    public class CreateAccountDTO : BaseValidationModel<CreateAccountDTO>
    {
        public int Balance {get; set;}
    }
}