using Banking.Domain.Contracts;
using Banking.Persistance;
using Banking.Persistance.Entities;
using Microsoft.EntityFrameworkCore;

namespace Banking.API.Extensions
{
    public static class AppBuilderExtensions
        {
            public static async Task Seed(this IApplicationBuilder applicationBuilder, IHostEnvironment environment) 
            {
                using (var scope = applicationBuilder.ApplicationServices.CreateScope()) 
                {
                    using(var context = scope.ServiceProvider.GetService<BankingDbContext>()) 
                    {
                        if(context is null) 
                        {
                            Console.WriteLine($"{nameof(context)} is null. Seeding cannot be instantiated");
                            return;
                        }

                        context.Database.Migrate();

                        if(!environment.IsProduction()) 
                        {
                            var authenticationService = scope.ServiceProvider
                                .GetRequiredService<IAuthenticationService>();

                            var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

                            context.SeedDatabase<CustomRole>("CustomRoles.json");
                            await context.SaveChangesAsync();

                            await context.SeedUser(authenticationService, "CustomUsers.json");
                            await context.SaveChangesAsync();
                            await context.SeedAccount(accountService, "Accounts.json");
                            await context.SaveChangesAsync();
                            await context.SeedBill(accountService);
                        }

                    }
                }
            }

        }
}