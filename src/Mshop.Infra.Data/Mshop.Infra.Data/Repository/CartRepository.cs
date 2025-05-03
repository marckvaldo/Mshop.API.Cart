using MongoDB.Driver;
using Mshop.Domain.Entity;
using Mshop.Infra.Data.Context;
using Mshop.Infra.Data.Interface;

namespace Mshop.Infra.Data.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly IMongoCollection<Cart> _carts;

        public CartRepository(MongoDbContext context)
        {
            _carts = context.Carts;
        }

        public async Task<Cart> GetByIdAsync(Guid id)
        {
            return await _carts.Find(cart => cart.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _carts.Find(cart => true).ToListAsync();
        }

        public async Task AddAsync(Cart cart, CancellationToken cancellationToken)
        {
            await _carts.InsertOneAsync(cart, cancellationToken:cancellationToken);
        }

        public async Task UpdateAsync(Cart cart, CancellationToken cancellationToken)
        {
            await _carts.ReplaceOneAsync(c => c.Id == cart.Id, cart, cancellationToken: cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _carts.DeleteOneAsync(cart => cart.Id == id, cancellationToken: cancellationToken);
        }

        public async Task<Cart> GetByCustomerId(Guid id)
        {
            return await _carts.Find(cart=>cart.Customer.Id == id).FirstOrDefaultAsync();
        }
    }
}
