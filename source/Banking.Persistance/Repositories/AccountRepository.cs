using Banking.Persistance.Contracts;
using Banking.Persistance.Entities;
using Banking.Shared.Exceptions;
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

        public void CreateAccount(Account account)
        {
            _context.Accounts.Add(account);
        }

        public async Task<Account> GetAccountById(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);

            if(account == null) throw new AccountNotFound(accountId);

            return account;
        }
    }
}