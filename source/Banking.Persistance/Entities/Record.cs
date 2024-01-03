
namespace Banking.Persistance.Entities
{
    public class Record
    {
        public int Id {get; set;}

        public DateTime TimeStamp {get; set;}

        public OperationType OperationType {get; set;}

        public int Amount {get; set;}

        public int UserId {get; set;}

        public int? AccountId {get; set;}

        public int? ReceiverAccountId {get; set;}

        public bool IsSuccessfull {get; set;}

        public string? ErrorMessage {get; set;}

        public bool IsPending {get; set;}

        // Navigation Properties

        public Account? Account {get; set;}

    }

    public enum OperationType 
    {
        CreateAccount,
        Payment,
        Deposit,
        Withdrawal,
        Transfer,
        CreditApplication,
        AutomaticPaymentSetup,
        SupportRequest
    }
}

