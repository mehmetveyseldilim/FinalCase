using Banking.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Persistance.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");

            builder.HasKey(key => key.Id);
            builder.Property(key => key.Id).HasColumnName("Id");

            builder.Property(a => a.Balance).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();
            builder.Property(a => a.UserId).IsRequired();
            builder.Property(a => a.DailySpend).IsRequired();
            builder.Property(a => a.DailyLimit).IsRequired();
            builder.Property(a => a.OperationLimit).IsRequired();

            builder.HasOne(a => a.User)
                    .WithMany(a => a.Accounts)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.Records)
                    .WithOne(a => a.Account)
                    .HasForeignKey(a => a.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);

            
            builder.Property<uint>("Version").IsRowVersion();

        }
    }
}