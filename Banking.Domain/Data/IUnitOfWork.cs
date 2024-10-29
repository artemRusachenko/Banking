using System.Data;

namespace Banking.Domain.Data
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        IDbTransaction BeginTransaction();
    }
}
