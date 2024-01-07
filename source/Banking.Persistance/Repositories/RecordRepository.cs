

using Banking.Persistance.Contracts;
using Banking.Persistance.Entities;
using Banking.Persistance.Extensions;
using Banking.Shared.RequestParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Banking.Persistance.Repositories
{
    public class RecordRepository : IRecordRepository
    {
        private readonly ILogger<RecordRepository> _logger;

        private readonly BankingDbContext _context;

        public RecordRepository(BankingDbContext context, ILogger<RecordRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void CreateRecord(Record record)
        {
            _context.Records.Add(record);
        }

        public async Task<IEnumerable<Record>> GetRecordsForUserAsync(int userId)
        {
            IEnumerable<Record> records = await _context.Records.Where(record => record.UserId == userId).ToListAsync();

            return records;
        }

        public async Task<IEnumerable<Record>> GetAllRecordsAsync(RecordParameters recordParameters)
        {
            IEnumerable<Record> records = await _context.Records
                                        .FilterRecords(recordParameters)
                                        .Sort(recordParameters.OrderBy!)
                                        .ToListAsync();

            return records;
        }

        public async Task<Record?> GetRecordById(int recordId)
        {
            Record? record = await _context.Records.Where(r => r.Id == recordId).SingleOrDefaultAsync();

            return record;
        }

        public async Task<Record?> GetPendingRecordById(int recordId)
        {
            Record? record = await _context.Records.Where(r => r.Id == recordId && r.IsPending == true).SingleOrDefaultAsync();

            return record;
        }
    }
}