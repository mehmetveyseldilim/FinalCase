using Banking.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Persistance.Configurations
{
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.ToTable("Bills"); 

            builder.HasKey(b => b.Id); 

            builder.Property(b => b.Amount)
                .IsRequired(); 

            builder.Property(b => b.LastPayTime)
                .IsRequired(); 

            builder.Property(b => b.IsActive)
                .IsRequired(); 

            // Configure the relationship with the Account entity
            builder.HasOne(b => b.Account)
                .WithMany(a => a.Bills)
                .HasForeignKey(b => b.AccountId)
                .OnDelete(DeleteBehavior.Restrict); 

        }
    }
}