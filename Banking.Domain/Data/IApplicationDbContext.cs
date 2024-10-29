using Banking.Domain.Accounts;
using Banking.Domain.Transactions;
using Banking.Domain.Transfers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Banking.Domain.Data
{
    public interface IApplicationDbContext
    {
        DbSet<Account> Accounts { get; set; }
        DbSet<Transaction> Transaction { get; set; }
        DbSet<Transfer> Transfer { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
