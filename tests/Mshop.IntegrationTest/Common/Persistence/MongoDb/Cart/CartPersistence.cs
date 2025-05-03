using MongoDB.Driver;
using Entity = Mshop.Domain.Entity;
using Mshop.Infra.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Mshop.Domain.Entity;

namespace Mshop.IntegrationTest.Common.Persistence.MongoDb.Cart
{
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
    }
}
