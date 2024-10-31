using System;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Domain.Transactions;
using Banking.Domain.Transfers;
using Banking.Infrastructure.Data;
using Banking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Banking.IntegrationTests
{
    public class IntegrationTestBase : IDisposable
    {
        internal readonly ApplicationDbContext _context;
        internal readonly IAccountRepository _accountRepository;
        internal readonly ITransactionRepository _transactionRepository;
        internal readonly ITransferRepository _transferRepository;
        internal readonly IUnitOfWork _unitOfWork;
        private bool _disposed;

        public IntegrationTestBase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _accountRepository = new AccountRepository(_context);
            _transactionRepository = new TransactionRepository(_context);
            _transferRepository = new TransferRepository(_context);
            _unitOfWork = new UnitOfWork(_context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    (_context)?.Database.CloseConnection();
                    (_context!)?.Dispose();
                }

                _disposed = true;
            }
        }
    }
}