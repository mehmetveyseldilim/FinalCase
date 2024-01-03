
using Banking.Persistance.Entities;

namespace Banking.Domain.Contracts
{
    public interface IErrorRecordHandler
    {
        void AddErrorRecord(Record record);
    }
}