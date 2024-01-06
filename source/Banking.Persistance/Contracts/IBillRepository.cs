
using Banking.Persistance.Entities;

namespace Banking.Persistance.Contracts
{
    public interface IBillRepository
    {
        void CreatBill(Bill bill);
    }
}