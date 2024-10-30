using Azure.Core;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Banking.Infrastructure.Repositories
{
    public class AccountRepository(IApplicationDbContext context) : IAccountRepository
    {
        public async Task AddAsync(Account account)
        {
            ArgumentNullException.ThrowIfNull(context.Accounts);

            await context.Accounts.AddAsync(account).ConfigureAwait(false);
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

        public Task Deposit(Account account, decimal amount)
        {
            ArgumentNullException.ThrowIfNull(account);

            account.Balance += amount;
            context.Accounts.Update(account);

            return Task.CompletedTask;
        }

        public Task Withdraw(Account account, decimal amount)
        {
            ArgumentNullException.ThrowIfNull(account);

            account.Balance -= amount;
            context.Accounts.Update(account);

            return Task.CompletedTask;
        }

        public Task Transfer(Account fromAccount, Account toAccount, decimal amount)
        {
            ArgumentNullException.ThrowIfNull(fromAccount);
            ArgumentNullException.ThrowIfNull(toAccount);

            fromAccount.Balance -= amount;
            context.Accounts.Update(fromAccount);

            toAccount.Balance += amount;
            context.Accounts.Update(toAccount);

            return Task.CompletedTask;
        }
    }
}
