
namespace Banking.Shared.DTOs.Request
{
    public class CreateBillDTO : BaseValidationModel<CreateBillDTO>
    {
        public int Amount {get; set;}

        public DateTime LastPayTime {get; set;}

        public int AccountId {get; set;}

    }
}