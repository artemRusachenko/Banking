using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Domain.Transactions;
using Banking.Domain.Transfers;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Transfer> Transfer { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}
