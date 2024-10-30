using Banking.Domain.Data;
using Banking.Domain.Transfers;
using Banking.Infrastructure.Data;

namespace Banking.Infrastructure.Repositories
{
    public class TransferRepository(IApplicationDbContext applicationDbContext) : ITransferRepository
    {
        public async Task AddAsync(Transfer transfer)
        {
            ArgumentNullException.ThrowIfNull(transfer);

            await applicationDbContext.Transfers.AddAsync(transfer).ConfigureAwait(false);
        }
    }
}
