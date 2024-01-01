using Banking.Persistance.Entities;

namespace Banking.Persistance.Contracts
{
    public interface IAccountRepository
    {
        void CreateAccount(Account account);

        Task<Account> GetAccountById(int accountId);
    }
}