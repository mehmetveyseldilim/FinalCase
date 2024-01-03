using Banking.Persistance.Configurations;
using Banking.Persistance.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Banking.Persistance 
{
    public class BankingDbContext : IdentityDbContext<CustomUser, CustomRole, int,
    IdentityUserClaim<int>, CustomUserRole, IdentityUserLogin<int>,  
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public const string SCHEMA_NAME = "BankingSchema";
        public DbSet<Account> Accounts {get; set;}

        public DbSet<Record> Records {get; set;}



        public BankingDbContext(DbContextOptions<BankingDbContext> dbContextOptions) : base(dbContextOptions)
        {
            // Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.HasDefaultSchema(SCHEMA_NAME);

            modelBuilder.ApplyConfiguration(new CustomUserConfiguration());
            modelBuilder.ApplyConfiguration(new CustomRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
            modelBuilder.ApplyConfiguration(new RecordConfiguration());


        }

    }
}




