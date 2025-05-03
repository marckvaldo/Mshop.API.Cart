using Mshop.Domain.Entity;

namespace Mshop.Infra.Data.Interface
{
    public interface ICartRepository
    {
        Task<Cart> GetByIdAsync(Guid id);
        Task<IEnumerable<Cart>> GetAllAsync();
        Task AddAsync(Cart cart, CancellationToken cancellationToken);
        Task UpdateAsync(Cart cart, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<Cart> GetByCustomerId(Guid id);
    }
}
