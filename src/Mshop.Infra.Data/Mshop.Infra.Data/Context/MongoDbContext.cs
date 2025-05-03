using MongoDB.Driver;
using Mshop.Domain.Entity;

namespace Mshop.Infra.Data.Context
{
    public class MongoDbContext
    {
        public readonly IMongoDatabase Database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Cart> Carts => Database.GetCollection<Cart>("Carts");
    }
}
