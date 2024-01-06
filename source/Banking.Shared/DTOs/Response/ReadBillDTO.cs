

namespace Banking.Shared.DTOs.Response
{
    public class ReadBillDTO
    {
        public int Id {get; set;}

        public int Amount {get; set;}

        public DateTime LastPayTime {get; set;}

        public bool IsActive {get; set;}

        public int AccountId {get; set;}

    }
}