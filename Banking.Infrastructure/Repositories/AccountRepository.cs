using Banking.Domain.Accounts;
using Banking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Repositories
{
    public class AccountRepository(ApplicationDbContext context) : IAccountRepository
    {
        public Task AddAsync(Account account)
        {
            ArgumentNullException.ThrowIfNull(context.Accounts);

            context.Accounts.Add(account);

            return Task.CompletedTask;
        }

        public async Task<IEnumerable<Account>> GetAllAsync()
        {
            ArgumentNullException.ThrowIfNull(context.Accounts);

            var accounts = context.Accounts;

            return await accounts.ToListAsync().ConfigureAwait(false);
        }

        public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
        {
            ArgumentNullException.ThrowIfNull(context.Accounts);

            var account = await context.Accounts.FirstOrDefaultAsync(a => a.AccountNumber == accountNumber).ConfigureAwait(false);

            return account;
        }
    }
}
