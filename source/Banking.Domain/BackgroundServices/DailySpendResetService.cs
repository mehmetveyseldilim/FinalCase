
using Banking.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Banking.Domain.BackgroundServices
{
    [DisallowConcurrentExecution]
    public class DailySpendResetService : IJob
    {
        private readonly ILogger<DailySpendResetService> _logger;
        private readonly IDbContextFactory<BankingDbContext> _contextFactory;
        public DailySpendResetService(ILogger<DailySpendResetService> logger, IDbContextFactory<BankingDbContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Processing daily spend reset at midnight...");
            using var dbContext = _contextFactory.CreateDbContext();
            using var transaction = dbContext.Database.BeginTransaction();
            try
            {
                // Reset daily spend for all accounts
                await dbContext.Accounts.ForEachAsync(account => account.DailySpend = 0);
                await dbContext.SaveChangesAsync();
                _logger.LogInformation("Daily spend reset successfully completed.");
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Error during daily spend reset: {message}", ex.Message);
            }
        }
    }
}