
namespace Banking.Persistance.Entities
{
    public class Bill
    {
        public int Id {get; set;}

        public int Amount {get; set;}

        public DateTime LastPayTime {get; set;}

        public bool IsActive {get; set;}

        public int AccountId {get; set;}

        public virtual Account? Account {get; set;}
    }
}