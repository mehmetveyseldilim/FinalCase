using Banking.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Persistance.Configurations
{
    public class CustomUserConfiguration : IEntityTypeConfiguration<CustomUser>
    {
        public void Configure(EntityTypeBuilder<CustomUser> builder)
        {
            builder
                .HasMany(e => e.UserRoles)  
                .WithOne(e => e.User)  
                .HasForeignKey(ur => ur.UserId)  
                .IsRequired();


            builder
                .HasOne(u => u.Account)
                .WithOne(a => a.User)
                .HasForeignKey<Account>(a => a.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}