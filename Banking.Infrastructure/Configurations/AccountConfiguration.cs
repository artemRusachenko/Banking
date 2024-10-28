using Banking.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Banking.Infrastructure.Configurations
{
    public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(x => x.Id);

            builder.Property(a => a.Balance)
                .HasColumnType("decimal(18, 2)")
                .IsRequired();

            builder.Property(a => a.HolderName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.AccountNumber)
                .IsRequired();
            //.HasLength(20);

            builder.HasMany(a => a.Transactions)
                .WithOne(a => a.Account)
                .HasForeignKey(t => t.AccountId);

            builder.HasMany(a => a.TransferFrom)
                .WithOne(t => t.FromAccount)
                .HasForeignKey(t => t.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.TransferTo)
                .WithOne(t => t.ToAccount)
                .HasForeignKey(t => t.ToAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
