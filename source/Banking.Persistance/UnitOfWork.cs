using System.Data;
using Banking.Persistance.Contracts;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Banking.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly BankingDbContext _context;
        private readonly ILogger<UnitOfWork> _logger;


        public UnitOfWork(BankingDbContext context, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();

            return transaction.GetDbTransaction();
        }

        

        public void Dispose()
        {
            _context.Dispose();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void ClearChangeTracker()
        {
            _context.ChangeTracker.Clear();
        }
    }
}