using Mshop.Core.DomainObject;
using Mshop.Core.Paginated;
using System.Linq.Expressions;

namespace Mshop.Core.Data
{
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity
    {
        public Task Create(TEntity entity, CancellationToken cancellationToken);
        public Task Update(TEntity entity, CancellationToken cancellationToken);
        public Task DeleteById(TEntity entity, CancellationToken cancellationToken);
        public Task<TEntity?> GetById(Guid Id);
        public Task<List<TEntity>> GetValuesList();
        public Task<List<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetLastRegister(Expression<Func<TEntity, bool>> predicate);
        Task<PaginatedOutPut<TEntity>> FilterPaginated(PaginatedInPut input, CancellationToken cancellationToken);
        Task<int> SaveChanges();
    }
}
