using Mshop.Core.DomainObject;

namespace Mshop.Core.Paginated
{
    public interface IPaginated<TEntity> where TEntity : Entity
    {
        Task<PaginatedOutPut<TEntity>> FilterPaginated(PaginatedInPut input);
    }
}
