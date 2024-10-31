using Banking.Domain.Data;

namespace Banking.Infrastructure.Data
{
    public class UnitOfWork(IApplicationDbContext context) : IUnitOfWork
    {
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
