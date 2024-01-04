
using Banking.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Banking.Domain.BackgroundServices
{
    [DisallowConcurrentExecution]
    public class PayBillsBackgroundService : IJob
    {

        private readonly ILogger<DailySpendResetService> _logger;
        private readonly IDbContextFactory<BankingDbContext> _contextFactory;

        public PayBillsBackgroundService(ILogger<DailySpendResetService> logger, 
        IDbContextFactory<BankingDbContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }


        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Processing Bills");

            using (var dbContext = _contextFactory.CreateDbContext())
            {
                try
                {
                    _logger.LogDebug("Getting all bills that has today's date");
                    // Reset daily spend for all accounts
                    await dbContext.Accounts.ForEachAsync(account => account.DailySpend = 0);
                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation("Daily spend reset successfully completed.");

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during daily spend reset: {message}", ex.Message);
                }


            }
        }
    }
}