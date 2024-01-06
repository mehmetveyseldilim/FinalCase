using Banking.API.Helper;
using Banking.Domain.Contracts;
using Banking.Persistance.Entities;
using Banking.Shared.DTOs.Request;
using Microsoft.EntityFrameworkCore;

namespace Banking.API.Extensions
{
public static class DbContextExtensions
    {
        public static void SeedDatabase<T>(this DbContext context, string filePath) where T : class
        {
            if(!context.Set<T>().Any()) 
            {
                var objects = SeedHelper.SeedData<T>(filePath);
                context.Set<T>().AddRange(objects);
            }
        }

        public static async Task SeedUser(this DbContext context, IAuthenticationService service, string filePath)
        {
            if(!context.Set<CustomUser>().Any())
            {
                var users = SeedHelper.SeedData<CreateUserDTO>(filePath);

                foreach(var user in users)
                {
                    await service.RegisterUser(user);

                }
            }
        }

        public static async Task SeedAccount(this DbContext context, IAccountService service, string filePath)
        {
            if(!context.Set<Account>().Any())
            {
                var accounts = SeedHelper.SeedData<CreateAccountDTO>(filePath);
                   
                await service.CreateAccountsAsync(accounts);
                
            }


        }

        public static async Task SeedBill(this DbContext context, IAccountService service)
        {
            if(!context.Set<Bill>().Any())
            {
                var bills = new List<CreateBillDTO>()
                {
                    new CreateBillDTO() { AccountId = 2, Amount = 35, LastPayTime = DateTime.UtcNow.AddMinutes(2)},
                    new CreateBillDTO() {AccountId = 2, Amount = 45, LastPayTime = DateTime.UtcNow.AddDays(1)},
                    new CreateBillDTO() {AccountId = 2, Amount = 105, LastPayTime = DateTime.UtcNow.AddMinutes(20)},
                    new CreateBillDTO() {AccountId = 3, Amount = 60, LastPayTime = DateTime.UtcNow.AddHours(5)}
                };
                   
                await service.CreateBillsAsync(bills);
            }

        }

        
    }
}