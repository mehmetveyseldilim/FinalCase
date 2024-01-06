using Banking.Persistance;
using Banking.Test.Fixtures;

namespace Banking.Test.Integration.AccountsOperations
{
    [CollectionDefinition("Account Operations")]
    public class AccountOperationsCollection : ICollectionFixture<IntegrationTestFactory<Program, BankingDbContext>>
    {
        
    }
}