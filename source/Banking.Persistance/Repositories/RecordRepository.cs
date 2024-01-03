

using Banking.Persistance.Contracts;
using Banking.Persistance.Entities;
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
            _logger.LogDebug("In {@className} {@methoName} method", nameof(RecordRepository), nameof(CreateRecord));
            _context.Records.Add(record);
            _logger.LogDebug("Added record: {@record}", record);
        }
    }
}