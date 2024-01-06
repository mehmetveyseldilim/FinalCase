

using Banking.Persistance;
using Banking.Test.Fixtures;

namespace Banking.Test.Integration.UserOperations
{
    [CollectionDefinition("User Operations")]
    public class UserOperationsCollection : ICollectionFixture<IntegrationTestFactory<Program, BankingDbContext>>
    {
        
    }
}