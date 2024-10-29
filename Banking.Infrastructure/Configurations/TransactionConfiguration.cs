using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Banking.Domain.Transactions;

namespace Banking.Infrastructure.Configurations
{
    public sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Description)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            builder.Property(t => t.TransactionDate)
                .IsRequired();

            builder.HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId);
        }
    }
}
