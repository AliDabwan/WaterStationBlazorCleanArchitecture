using System;
using System.Threading;
using System.Threading.Tasks;
using WaterS.Domain.Contracts;

namespace WaterS.Application.Interfaces.Repositories
{
    public interface IUnitOfWork<TId> : IDisposable
    {
        IRepositoryAsync<T, TId> Repository<T>() where T : AuditableEntity<TId>;

        Task<int> Commit(CancellationToken cancellationToken);
        Task<int> Commit();

        Task<int> CommitAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();
    }
}