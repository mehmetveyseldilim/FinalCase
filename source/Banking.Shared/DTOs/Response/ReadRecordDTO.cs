
namespace Banking.Shared.DTOs.Response
{
    public class ReadRecordDTO
    {
        public int Id {get; set;}

        public DateTime TimeStamp {get; set;}

        public string? OperationType {get; set;}

        public int Amount {get; set;}

        public int UserId {get; set;}

        public int? AccountId {get; set;}

        public int? ReceiverAccountId {get; set;}

        public bool IsSuccessfull {get; set;}

        public string? ErrorMessage {get; set;}

        public bool IsPending {get; set;}

    }
}