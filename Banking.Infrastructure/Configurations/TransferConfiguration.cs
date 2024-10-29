using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Banking.Domain.Transfers;

namespace Banking.Infrastructure.Configurations
{
    public class TransferConfiguration : IEntityTypeConfiguration<Transfer>
    {
        public void Configure(EntityTypeBuilder<Transfer> builder)
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(t => t.TransferDate)
                .IsRequired();

            builder.HasOne(t => t.FromAccount)
                .WithMany(a => a.TransferFrom)
                .HasForeignKey(t => t.FromAccountId);

            builder.HasOne(t => t.ToAccount)
                .WithMany(a => a.TransferTo)
                .HasForeignKey(t => t.ToAccountId);
        }
    }
}
