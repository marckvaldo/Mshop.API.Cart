using Dapper;
using Mshop.Core.Common;
using Mshop.Core.DomainObject;
using Mshop.IntegrationTest.Common.Persistence.DTOs;
using MySql.Data.MySqlClient;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mshop.Infra.Consumer.Cache;

namespace Mshop.IntegrationTest.Common.Persistence.Cache.Product
{
    public class ProductPersistenceCache
    {
        private readonly IDatabase _database;
        private readonly SearchCommands _search;
        private readonly string _indexName = "ProductIndex";
        private readonly string _keyPrefix = "Product:";

        public ProductPersistenceCache(IConnectionMultiplexer database)
        {
            _database = database.GetDatabase();
            _search = _database.FT();

            _indexName = $"{IndexName.Product}Index";
            _keyPrefix = $"{IndexName.Product}:";
        }

        

        public async Task<ProductsPersistenceDTO> GetProductByIdAsync(Guid productId)
        {
            var key = $"{_keyPrefix}{productId}";
            var hash = await _database.HashGetAllAsync(key);

            if (hash.Length == 0)
                return null;

            return RedisToProduct(hash);
        }

        public async Task<IEnumerable<ProductsPersistenceDTO>> GetAllProductsAsync()
        {
            // Cria uma consulta que busca todos os documentos do índice
            var query = new Query("*");

            // Executa a busca no índice RedisSearch
            var result = await _search.SearchAsync(_indexName, query);

            // Se nenhum documento for encontrado, retorna uma lista vazia
            if (result.Documents.Count == 0)
                return Enumerable.Empty<ProductsPersistenceDTO>();

            // Converte os documentos do Redis em objetos ProductsPersistenceDTO
            var products = result.Documents
                                 .Select(doc => RedisToProduct(doc))
                                 .ToList();

            return products;
        }

        public async Task AddProductAsync(ProductsPersistenceDTO product, DateTime? ExpirationDate)
        {
            var key = $"{_keyPrefix}{product.Id}";

            var hash = new HashEntry[]
            {
                new("Id", product.Id.ToString()),
                new("Name", product.Name),
                new("Description", product.Description),
                new("Price", product.Price.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"))),
                new("Stock", product.Stock.ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"))),
                new("IsActive", product.IsActive),
                new("IsSale", product.IsSale),
                new("CategoryId", Helpers.ClearString(product.CategoryId.ToString())),
                new("Category", ""),
                new("Thumb", product.Thumb)
            };

            await _database.HashSetAsync(key, hash);

            if (ExpirationDate.HasValue)
            {
                await _database.KeyExpireAsync(key, ExpirationDate.Value);
            }
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var key = $"{_keyPrefix}{productId.ToString()}";
            return await _database.KeyDeleteAsync(key);
        }

        public async Task DeleteAllProductAsync()
        {
            try
            {
                // Cria uma consulta que busca todos os documentos do índice
                var query = new Query("*");

                // Busca todos os documentos no índice
                var result = await _search.SearchAsync(_indexName, query);

                // Itera sobre cada documento retornado
                foreach (var document in result.Documents)
                {
                    // Cada documento tem uma chave única, que pode ser usada para exclusão
                    await _database.KeyDeleteAsync(document.Id);
                }
            }
            catch (StackExchange.Redis.RedisServerException ex)
            {
                // Trate a exceção conforme necessário
                //throw new Exception("Erro ao deletar todos os produtos do Redis", ex);

            }
        }







        private ProductsPersistenceDTO RedisToProduct(HashEntry[] hash)
        {
            bool isActive = hash.FirstOrDefault(x => x.Name == "IsActive").Value.ToString() == "1" ? true : false;
            bool isPromotion = hash.FirstOrDefault(x => x.Name == "IsSales").Value.ToString() == "1" ? true : false;

            var product = new ProductsPersistenceDTO();
            product.Description = hash.FirstOrDefault(x => x.Name == "Description").Value.ToString() ?? string.Empty;
            product.Name = hash.FirstOrDefault(x => x.Name == "Name").Value.ToString() ?? string.Empty;
            product.Price = decimal.Parse(hash.FirstOrDefault(x => x.Name == "Price").Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
            product.CategoryId = Guid.Parse(hash.FirstOrDefault(x => x.Name == "CategoryId").Value.ToString());
            product.Stock = decimal.Parse(hash.FirstOrDefault(x => x.Name == "Stock").Value.ToString(), System.Globalization.CultureInfo.InvariantCulture);
            product.IsActive = isActive;
            product.Id = Guid.Parse(hash.FirstOrDefault(x => x.Name == "Id").Value.ToString());
            product.IsSale = isPromotion;
            product.Thumb = hash.FirstOrDefault(x => x.Name == "Thumb").Value.ToString() ?? string.Empty;
            
            return product;
        }

        private ProductsPersistenceDTO RedisToProduct(Document doc)
        {
            bool isActive = doc["IsActive"].ToString() == "1" ? true : false;
            bool isPromotion = doc["IsSale"].ToString() == "1" ? true : false;

            var product = new ProductsPersistenceDTO();
            product.Name = doc["Name"].ToString() ?? string.Empty;
            product.Description = doc["Description"].ToString() ?? string.Empty;
            product.Price = decimal.Parse(doc["Price"].ToString(), System.Globalization.CultureInfo.InvariantCulture);
            product.CategoryId = Guid.Parse(doc["CategoryId"].ToString() ?? string.Empty);
            product.Stock = decimal.Parse(doc["Stock"].ToString() ?? "0");
            product.IsActive = isActive;
            product.Id = Guid.Parse(doc["Id"].ToString() ?? string.Empty);
            product.IsSale = isPromotion;
            product.Thumb = doc["Thumb"].ToString() ?? string.Empty;
                
            return product;

        }
    }
}
