
using Banking.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Persistance.Configurations
{
    public class CustomUserConfiguration : IEntityTypeConfiguration<CustomUser>
    {
        public void Configure(EntityTypeBuilder<CustomUser> builder)
        {
            // Each User can have many entries in the UserRole join table  

            builder.HasMany(e => e.UserRoles)  
                    .WithOne(e => e.User)  
                    .HasForeignKey(ur => ur.UserId)  
                    .IsRequired();


            builder.HasMany(u => u.Accounts)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);


        }
    }
}