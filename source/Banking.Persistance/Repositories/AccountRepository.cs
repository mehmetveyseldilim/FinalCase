using Banking.Persistance.Contracts;
using Banking.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Banking.Persistance.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ILogger<AccountRepository> _logger;

        private readonly BankingDbContext _context;

        public AccountRepository(BankingDbContext context, ILogger<AccountRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IUnitOfWork UnitOfWork => throw new NotImplementedException();

        public void CreateAccount(Account account)
        {
            _context.Accounts.Add(account);
        }

        public async Task<Account?> GetAccountById(int accountId)
        {
            var account = await _context.Accounts.Where(a => a.Id == accountId).SingleOrDefaultAsync();

            
            return account;
        }

        public async Task<Account?> GetAccountByUserId(int userId)
        {
            var account = await _context.Accounts
                                .Where(p => p.UserId == userId)
                                .FirstOrDefaultAsync();

            return account;
        }

        public async Task<Account?> GetAccountInformationsByUserId(int userId)
        {
            var account = await _context.Accounts.Where(a => a.UserId == userId)
                                .Include(account => account.Bills)
                                .SingleOrDefaultAsync();

            return account;


        }
    }
}