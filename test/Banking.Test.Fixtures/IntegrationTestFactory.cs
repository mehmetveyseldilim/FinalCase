using Banking.Persistance;
using Banking.Test.Fixtures.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace Banking.Test.Fixtures
{
    public class IntegrationTestFactory<TProgram, TDbContext> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class where TDbContext : DbContext 
    {
        private readonly PostgreSqlContainer _container;

        public IntegrationTestFactory()
        {
            _container = new PostgreSqlBuilder()
            .WithImage("postgres:15.5")
            .WithDatabase("BankingDb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(5432, true)
            .WithCleanUp(true)
            .Build();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var connectionString = _container.GetConnectionString();
            builder
                .UseEnvironment("Development")
                .ConfigureTestServices(services =>
                {
                    services.RemoveDbContext<TDbContext>();
                    services.AddDbContextFactory<TDbContext>(options => 
                    { 
                        options.UseNpgsql(connectionString, b =>
                            b.MigrationsAssembly("Banking.API")
                                .MigrationsHistoryTable("__EFMigrationsHistory", schema: BankingDbContext.SCHEMA_NAME));
                    });
                    services.EnsureDbCreated<TDbContext>();
                });
        }

        public async Task InitializeAsync() => await _container.StartAsync();

        public new async Task DisposeAsync() => await _container.DisposeAsync();
    }
}