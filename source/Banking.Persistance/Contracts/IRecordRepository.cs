using Banking.Persistance.Entities;

namespace Banking.Persistance.Contracts
{
    public interface IRecordRepository
    {
        void CreateRecord(Record record);
    }
}