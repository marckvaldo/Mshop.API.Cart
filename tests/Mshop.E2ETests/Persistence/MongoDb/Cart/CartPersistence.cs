using MongoDB.Driver;
using Mshop.Infra.Data.Context;
using Entity = Mshop.Domain.Entity;

namespace Mshop.E2ETests.Persistence.MongoDb.Product;

public class CartPersistence
{
    private readonly IMongoCollection<Entity.Cart> _carts;

    public CartPersistence(MongoDbContext context)
    {
        _carts = context.Carts;
    }

    public async Task DeleteAllCartAsync(CancellationToken cancellationToken)
    {
        await _carts.DeleteOneAsync(Builders<Entity.Cart>.Filter.Empty, cancellationToken: cancellationToken);
    }
    /*public async Task AddCartAsync(Entity.Cart cart, CancellationToken cancellationToken)
    {
        await _carts.InsertOneAsync(cart, cancellationToken: cancellationToken);
    }*/
}
