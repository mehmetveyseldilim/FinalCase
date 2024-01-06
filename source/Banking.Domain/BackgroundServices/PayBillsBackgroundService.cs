
using Banking.Persistance;
using Banking.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Banking.Domain.BackgroundServices
{
    [DisallowConcurrentExecution]
    public class PayBillsBackgroundService : IJob
    {

        private readonly ILogger<PayBillsBackgroundService> _logger;
        private readonly IDbContextFactory<BankingDbContext> _contextFactory;

        public PayBillsBackgroundService(ILogger<PayBillsBackgroundService> logger, 
        IDbContextFactory<BankingDbContext> contextFactory)
        {
            _logger = logger;
            _contextFactory = contextFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Processing Bills");

            OperationType operationType = OperationType.Payment;

            using (var dbContext = _contextFactory.CreateDbContext())
            {
                using(var transaction = dbContext.Database.BeginTransaction()) 
                {
                    try
                    {
                        DateTime today = DateTime.UtcNow.Date;
                        DateTime tomorrow = today.AddDays(1);

                        _logger.LogInformation("Today's date is {@today}", today);
                        _logger.LogInformation("Tomorrow's date is {@tomorrow}", tomorrow);
                        _logger.LogDebug("Getting all bills that has today's date");

                        var accountsWithBillsToday = await dbContext.Accounts
                            .Include(a => a.Bills!.Where(b => b.IsActive == true && b.LastPayTime >= today && b.LastPayTime < tomorrow))
                            .Where(a => a.Bills.Count != 0)
                            .ToListAsync();

                        List<Record> records = new List<Record>(); 

                        foreach (var account in accountsWithBillsToday)
                        {
                            _logger.LogDebug($"Account ID: {account.Id}, Balance: {account.Balance}, DailySpend: {account.DailySpend}");
                            
                            foreach (var bill in account.Bills)
                            {
                                Record record = new Record()
                                {
                                    TimeStamp = DateTime.UtcNow,
                                    OperationType = operationType,
                                    Amount = bill.Amount,
                                    UserId = account.UserId,
                                    AccountId = account.Id,
                                    IsSuccessfull = true,
                                    IsPending = false
                                };


                                if(account.Balance < bill.Amount ) 
                                {
                                    _logger.LogWarning("The bill could not be paid for account with id {@accountId} Current balance: {@accountBalance} and Bill amount: {@amount}"
                                    ,account.Id, account.Balance, bill.Amount);
                                    record.IsPending = true;
                                    record.IsSuccessfull = false;
                                    records.Add(record);

                                    continue;
                                }

                                account.Balance -= bill.Amount;
                                bill.LastPayTime = bill.LastPayTime.AddMonths(1);
                                records.Add(record);

                                _logger.LogDebug("Account new balance: {accountBalance}", account.Balance);
                                _logger.LogDebug("Bill new date: {billDate}", bill.LastPayTime);

                                _logger.LogDebug($"  Bill ID: {bill.Id}, Amount: {bill.Amount}, LastPayTime: {bill.LastPayTime}");
                            }

                        }

                        dbContext.Records.AddRange(records);
                        await dbContext.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Error during daily spend reset: {message}", ex.Message);
                    }
                }
                

            }
        }

    }
}