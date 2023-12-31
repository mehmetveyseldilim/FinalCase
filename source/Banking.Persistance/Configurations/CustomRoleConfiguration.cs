using Banking.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Persistance.Configurations
{
    public class CustomRoleConfiguration : IEntityTypeConfiguration<CustomRole>
    {
        public void Configure(EntityTypeBuilder<CustomRole> builder)
        {
            // Each Role can have many entries in the UserRole join table  
            builder.HasMany(e => e.UserRoles)  
                .WithOne(e => e.Role)  
                .HasForeignKey(ur => ur.RoleId)  
                .IsRequired();  
        }
    }
}