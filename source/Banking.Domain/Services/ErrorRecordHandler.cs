using Banking.Domain.Contracts;
using Banking.Persistance;
using Banking.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Banking.Domain.Services
{
    public  class ErrorRecordHandler : IErrorRecordHandler
    {
        private readonly ILogger<ErrorRecordHandler> _logger;
        private readonly IDbContextFactory<BankingDbContext> _contextFactory;

        public ErrorRecordHandler(IDbContextFactory<BankingDbContext> contextFactory, ILogger<ErrorRecordHandler> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public void AddErrorRecord(Record record) 
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                if (context is null)
                {
                    _logger.LogError($"{nameof(context)} is null. Returning");
                    return;
                }

                context.Records.Add(record);
                context.SaveChanges();
                _logger.LogInformation("Record has been saved into the db. Record: {@record}", record);
            }

        }
    }
}