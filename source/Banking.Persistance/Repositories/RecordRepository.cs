

using Banking.Persistance.Contracts;
using Banking.Persistance.Entities;
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
    }
}