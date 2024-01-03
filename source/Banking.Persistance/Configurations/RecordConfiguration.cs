using Banking.Persistance.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Persistance.Configurations
{
    public class RecordConfiguration : IEntityTypeConfiguration<Record>
    {
        public void Configure(EntityTypeBuilder<Record> builder)
        {
            builder.ToTable("Records");
            builder.HasKey(key => key.Id);

            builder.Property(r => r.UserId).IsRequired();
            builder.Property(r => r.TimeStamp).IsRequired();
            builder.Property(r => r.OperationType).IsRequired();
            builder.Property(r => r.Amount).IsRequired();
            builder.Property(r => r.IsSuccessfull).IsRequired();
            builder.Property(r => r.ErrorMessage).HasMaxLength(255);
            // builder.Property(r => r.IsCompleted).IsRequired();
            builder.Property(r => r.IsPending).IsRequired();

            builder.HasOne(r => r.Account) // A record is associated with one account
                .WithMany(a => a.Records) // An account can have multiple records
                .HasForeignKey(r => r.AccountId)
                .OnDelete(DeleteBehavior.Restrict); // Adjust the delete behavior as needed


        }
    }
}