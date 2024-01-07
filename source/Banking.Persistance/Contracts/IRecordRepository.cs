using Banking.Persistance.Entities;
using Banking.Shared.RequestParameters;

namespace Banking.Persistance.Contracts
{
    public interface IRecordRepository
    {
        void CreateRecord(Record record);

        Task<IEnumerable<Record>> GetRecordsForUserAsync(int userId);

        Task<IEnumerable<Record>> GetAllRecordsAsync(RecordParameters recordParameters);

        Task<Record?> GetRecordById(int recordId);

        Task<Record?> GetPendingRecordById(int recordId);

    }
}