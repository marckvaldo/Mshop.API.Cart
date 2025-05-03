using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.Commons.DTO;
using Mshop.IntegrationTest.Common;
using Mshop.IntegrationTest.Common.Persistence.Cache;
using Mshop.IntegrationTest.Common.Persistence.Cache.Product;
using Mshop.IntegrationTest.Common.Persistence.DTOs;
using Mshop.IntegrationTest.Common.Persistence.Mysql.Category;
using Mshop.IntegrationTest.Common.Persistence.Mysql.Product;
using Mshop.IntegrationTest.Infra.configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.IntegrationTest.Infra.Consumers
{
    public class ProductConsumerTestFixture : IntegrationBaseFixture
    {
        protected readonly ProductPersistence _productPersistenceDabaBase;
        protected readonly CategoryPersistence _categoryPersistenceDataBase;
        protected readonly ProductPersistenceCache _productPersistenceCache;
        protected readonly IConnectionMultiplexer _ConnectionCache;
        private readonly DateTime _expirationDate;
       
        public ProductConsumerTestFixture() : base()
        {
            _productPersistenceDabaBase = new ProductPersistence(ConfigurationTests.ConnectionStringMysql);
            _categoryPersistenceDataBase = new CategoryPersistence(ConfigurationTests.ConnectionStringMysql);


            _ConnectionCache = _serviceProvider.GetRequiredService<IConnectionMultiplexer>();
            _productPersistenceCache = new ProductPersistenceCache(_ConnectionCache);
  

            _expirationDate = DateTime.UtcNow.AddMinutes(5);
        }

        public bool PersistirProcutsMysql()
        {
            
            var category = new CategoryPersistenceDTO { IsActive = true, Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1)[0] };
            _categoryPersistenceDataBase.AddCategoryAsync(category).Wait();
            
            var produtct = FakerProducts(10, category.Id);
            foreach(var produto in produtct)
            {
                var produtoDTO = new ProductsPersistenceDTO();
                produtoDTO.Description = produto.Description;
                produtoDTO.Name = produto.Name;
                produtoDTO.Price = produto.Price;
                produtoDTO.Stock = 10;
                produtoDTO.IsActive = true;
                produtoDTO.IsSale = produto.IsSale;
                produtoDTO.CategoryId = produto.CategoryId;
                produtoDTO.Thumb = produto.Thumb;
                produtoDTO.Id = produto.Id;
                _productPersistenceDabaBase.AddProductAsync(produtoDTO).Wait();
            }

            return true;
        }

        public Task<IEnumerable<ProductsPersistenceDTO>> GetAllProductsMysqlAsync()
        {
            return _productPersistenceDabaBase.GetAllProductsAsync();
        }

        public Task<ProductsPersistenceDTO> GetProductMysqlById(Guid productId)
        {
            return _productPersistenceDabaBase.GetProductByIdAsync(productId);
        }

        public async Task ClearDataBase()
        {
            _productPersistenceDabaBase.DeleteAllProductAsync().Wait();
            _categoryPersistenceDataBase.DeleteAllCategoryAsync().Wait();
        }



        //Cache
        public async Task<bool> PersistirProductsCache()
        {

           var produtct = FakerProducts(10);
            foreach (var produto in produtct)
            {
                var produtoDTO = new ProductsPersistenceDTO();
                produtoDTO.Description = produto.Description;
                produtoDTO.Name = produto.Name;
                produtoDTO.Price = produto.Price;
                produtoDTO.Stock = 10;
                produtoDTO.IsActive = true;
                produtoDTO.IsSale = produto.IsSale;
                produtoDTO.CategoryId = produto.CategoryId;
                produtoDTO.Thumb = produto.Thumb;
                produtoDTO.Id = produto.Id;
                await _productPersistenceCache.AddProductAsync(produtoDTO, _expirationDate);
            }

            return true;
        }
        public async Task<IEnumerable<ProductsPersistenceDTO>> GetAllProductsCacheAsync()
        {
            return await _productPersistenceCache.GetAllProductsAsync();
        }
        public Task<ProductsPersistenceDTO> GetProductCacheById(Guid productId)
        {
            return _productPersistenceCache.GetProductByIdAsync(productId);
        }
        public async Task<bool> ClearCache()
        {
            await _productPersistenceCache.DeleteAllProductAsync();
            return true;
        }
        protected async Task DeleteCache()
        {
            var database = _ConnectionCache.GetDatabase();
            await database.ExecuteAsync("FLUSHALL");
        }
        protected async Task<bool> CreateIndexCache()
        {
            var startIndex = new StartIndex(_ConnectionCache);
            await startIndex.CreateIndex();
            return true;
        }


        public async Task<bool> PersistirProductCacheAndMysql()
        {
            var category = new CategoryPersistenceDTO { IsActive = true, Id = Guid.NewGuid(), Name = _faker.Commerce.Categories(1)[0] };
            _categoryPersistenceDataBase.AddCategoryAsync(category).Wait();

            var produtct = FakerProducts(10, category.Id);
            foreach (var produto in produtct)
            {
                var produtoDTO = new ProductsPersistenceDTO();
                produtoDTO.Description = produto.Description;
                produtoDTO.Name = produto.Name;
                produtoDTO.Price = produto.Price;
                produtoDTO.Stock = 10;
                produtoDTO.IsActive = true;
                produtoDTO.IsSale = produto.IsSale;
                produtoDTO.CategoryId = produto.CategoryId;
                produtoDTO.Thumb = produto.Thumb;
                produtoDTO.Id = produto.Id;

                await _productPersistenceCache.AddProductAsync(produtoDTO, _expirationDate);

                await _productPersistenceDabaBase.AddProductAsync(produtoDTO);
            }

            return true;
        }
    }
}
