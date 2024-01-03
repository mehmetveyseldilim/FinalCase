
using System.Data;

namespace Banking.Persistance.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        void ClearChangeTracker();

        void SaveChanges();

        IDbTransaction BeginTransaction();



    }
}