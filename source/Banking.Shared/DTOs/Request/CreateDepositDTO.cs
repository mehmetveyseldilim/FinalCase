
namespace Banking.Shared.DTOs.Request
{
    public class CreateDepositDTO : BaseValidationModel<CreateDepositDTO>
    {
        public int Amount {get; set;}
    }
}