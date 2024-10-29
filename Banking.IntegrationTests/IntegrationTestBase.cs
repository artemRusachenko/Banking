using System;
using Banking.Domain.Accounts;
using Banking.Domain.Data;
using Banking.Infrastructure.Data;
using Banking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Banking.IntegrationTests
{
    public class IntegrationTestBase : IDisposable
    {
        internal readonly IApplicationDbContext _context;
        internal readonly IAccountRepository _accountRepository;
        internal readonly IUnitOfWork _unitOfWork;
        private bool _disposed;

        public IntegrationTestBase()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            var applicationDbContext = new ApplicationDbContext(options);
            _context = applicationDbContext;
            _accountRepository = new AccountRepository(applicationDbContext);
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
                    ((ApplicationDbContext)_context)?.Database.EnsureDeleted();
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
