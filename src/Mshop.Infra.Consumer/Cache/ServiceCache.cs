using Mshop.Infra.Consumer.DTOs;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Mshop.Infra.Consumer.Cache
{
    public class ServiceCache : IServiceCache
    {

        private readonly IDatabase _database;
        private readonly string _keyPrefix = "Product:";

        public ServiceCache(IConnectionMultiplexer database)
        {
            _database = database.GetDatabase();

            _keyPrefix = $"{IndexName.Product}:";
        }
        public async Task<ProductModel?> GetProductById(Guid id)
        {

            var key = $"{_keyPrefix}{id}";
            var hash = await _database.HashGetAllAsync(key);

            if (hash.Length == 0)
                return null;

            return RedisToProduct(hash);

        }



        private ProductModel RedisToProduct(HashEntry[] hash)
        {
            bool isActive = hash.FirstOrDefault(x => x.Name == "IsActive").Value.ToString() == "1" ? true : false;
            bool isPromotion = hash.FirstOrDefault(x => x.Name == "IsSale").Value.ToString() == "1" ? true : false;

            var product = new ProductModel(
                Description: hash.FirstOrDefault(x => x.Name == "Description").Value.ToString() ?? string.Empty,
                Name: hash.FirstOrDefault(x => x.Name == "Name").Value.ToString() ?? string.Empty,
                Price: decimal.Parse(hash.FirstOrDefault(x => x.Name == "Price").Value.ToString(), System.Globalization.CultureInfo.InvariantCulture),
                CategoryId: Guid.Parse(hash.FirstOrDefault(x => x.Name == "CategoryId").Value.ToString()),
                Id: Guid.Parse(hash.FirstOrDefault(x => x.Name == "Id").Value.ToString()),
                IsPromotion: isPromotion,
                Category : hash.FirstOrDefault(x => x.Name == "Category").Value.ToString(),
                Thumb: hash.FirstOrDefault(x => x.Name == "Thumb").Value.ToString()
                );

            return product;
        }
    }
}
