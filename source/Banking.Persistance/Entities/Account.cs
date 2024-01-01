
namespace Banking.Persistance.Entities
{
    public class Account
    {
        public int Id {get; set;}

        public int Balance {get; set;}

        public DateTime CreatedAt {get; set;}

        public int UserId {get; set;}

        public CustomUser? User {get; set;}

        public int DailySpend {get; set;}

        public int DailyLimit {get; set;}

        public int OperationLimit {get; set;}
    }
}