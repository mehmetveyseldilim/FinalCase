
namespace Banking.Shared.DTOs.Request
{
    public class CreateWithdrawDTO: BaseValidationModel<CreateDepositDTO>
    {
        public int Amount {get; set;}
        
    }

}