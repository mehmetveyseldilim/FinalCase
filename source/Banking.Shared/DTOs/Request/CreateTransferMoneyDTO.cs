
namespace Banking.Shared.DTOs.Request
{
    public class CreateTransferMoneyDTO : BaseValidationModel<CreateTransferMoneyDTO>
    {
        public int ReceiverAccountId {get; set;}

        public int Amount {get; set;}

    }
}