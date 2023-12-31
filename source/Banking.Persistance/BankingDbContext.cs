﻿using Banking.Persistance.Configurations;
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

        }
    }
}




