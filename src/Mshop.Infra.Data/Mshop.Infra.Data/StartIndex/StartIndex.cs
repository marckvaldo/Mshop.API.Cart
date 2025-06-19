using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Infra.Data.StartIndex
{
    public class StartIndex
    {
        private readonly IMongoCollection<Cart> _cartCollection;
        private readonly IMongoDatabase _database;

        public StartIndex(IMongoDatabase database)
        {
            _cartCollection = database.GetCollection<Cart>("Carts");
            _database = database;

        }

        public async Task CreateIndexes()
        {
            // Índice para Cart ID
            /*var cartIdIndex = Builders<Cart>.IndexKeys.Ascending(c => c.Id);
            var uniqueOptions = new CreateIndexOptions { Unique = true };
            await _cartCollection.Indexes.CreateOneAsync(new CreateIndexModel<Cart>(cartIdIndex, uniqueOptions));*/

            // Índice para Customer ID
            var customerIdIndex = Builders<Cart>.IndexKeys.Ascending(c => c.Customer.Id);
            await _cartCollection.Indexes.CreateOneAsync(new CreateIndexModel<Cart>(customerIdIndex));

            // Índice para Product ID (em uma lista)
            var productIdIndex = Builders<Cart>.IndexKeys.Ascending("Products.Id");
            await _cartCollection.Indexes.CreateOneAsync(new CreateIndexModel<Cart>(productIdIndex));
        }
        
        public async Task CrateCollection(string CollectionName)
        {
            if (!_database.ListCollectionNames().ToList().Contains(CollectionName))
                _database.CreateCollection(CollectionName);
        }

        public async Task DropCollection(string CollectionName)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "Development")
            {
                if (_database.ListCollectionNames().ToList().Contains(CollectionName))
                    await _database.DropCollectionAsync(CollectionName);
            }
        }
    }
}
