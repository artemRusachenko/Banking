using System;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Domain.Transactions;
using Banking.Infrastructure.Data;
using Banking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Banking.IntegrationTests
{
    public class IntegrationTestBase : IDisposable
    {
        internal readonly IApplicationDbContext _context;
        internal readonly IAccountRepository _accountRepository;
        internal readonly ITransactionRepository _transactionRepository;
        internal readonly IUnitOfWork _unitOfWork;
        private bool _disposed;

        public IntegrationTestBase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            var applicationDbContext = new ApplicationDbContext(options);
            applicationDbContext.Database.OpenConnection();
            applicationDbContext.Database.EnsureCreated();

            _context = applicationDbContext;
            _accountRepository = new AccountRepository(applicationDbContext);
            _transactionRepository = new TransactionRepository(applicationDbContext);
            _unitOfWork = new UnitOfWork(applicationDbContext);
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
                    ((ApplicationDbContext)_context)?.Database.CloseConnection();
                    ((ApplicationDbContext)_context!)?.Dispose();
                }

                _disposed = true;
            }
        }

        ~IntegrationTestBase()
        {
            Dispose(false);
        }
    }
}
