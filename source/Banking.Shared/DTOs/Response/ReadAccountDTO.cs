
namespace Banking.Shared.DTOs.Response
{
    public class ReadAccountDTO
    {
        public int Id {get; set;}

        public int Balance {get; set;}

        public DateTime CreatedAt {get; set;}

        public int UserId {get; set;}

        public int DailySpend {get; set;}

        public int DailyLimit {get; set;}

        public int OperationLimit {get; set;}

        public IEnumerable<ReadBillDTO>? Bills {get; set;}

    }
}