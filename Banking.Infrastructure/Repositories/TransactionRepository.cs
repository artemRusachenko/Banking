using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Domain.Transactions;

namespace Banking.Infrastructure.Repositories
{
    public class TransactionRepository(IApplicationDbContext applicationDbContext) : ITransactionRepository
    {
        public async Task AddAsync(Transaction transaction)
        {
            ArgumentNullException.ThrowIfNull(transaction);

            await applicationDbContext.Transactions.AddAsync(transaction).ConfigureAwait(false);
        }
    }
}
