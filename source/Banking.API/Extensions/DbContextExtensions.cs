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
    }
}