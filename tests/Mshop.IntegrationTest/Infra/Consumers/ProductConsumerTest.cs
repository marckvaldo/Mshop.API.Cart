using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.IntegrationTest.Infra.Consumers
{
    public class ProductConsumerTest : ProductConsumerTestFixture
    {
        //private readonly IServiceProvider _serviceProvider;
        private readonly IProductConsumer _productConsumer;
        public ProductConsumerTest() : base()
        {
            _productConsumer = _serviceProvider.GetRequiredService<IProductConsumer>();
            ClearDataBase().Wait();
            
            DeleteCache().Wait();
            CreateIndexCache().Wait();
        }

        [Fact(DisplayName = nameof(ProductConsumer_ShouldConsumeMessageFromGRPC))]
        [Trait("Integration - Infra.Consumer", "Product Consumer")]
        public async Task ProductConsumer_ShouldConsumeMessageFromGRPC()
        {
            // Arranges
            PersistirProcutsMysql();
            var products = await GetAllProductsMysqlAsync();
            var product = products.FirstOrDefault();

            var result = await _productConsumer.GetProductByIdAsync(product.Id);
            // Asserts
            Assert.NotNull(product);
            Assert.Equal(result.Name, product.Name);
            Assert.Equal(result.Description, product.Description);
            Assert.Equal(result.Price, product.Price);
            Assert.Equal(result.IsPromotion, product.IsSale);
            Assert.Equal(result.CategoryId, product.CategoryId);
            Assert.Equal(result.Thumb, product.Thumb);
            Assert.Equal(result.Id, product.Id);

        }


        [Fact(DisplayName = nameof(ProductConsumer_ShouldConsumeMessageFromCache))]
        [Trait("Integration - Infra.Consumer", "Product Consumer")]
        public async Task ProductConsumer_ShouldConsumeMessageFromCache()
        {
            // Arranges
            await PersistirProductsCache();
            var products = await GetAllProductsCacheAsync();
            var product = products.FirstOrDefault();

            var result = await _productConsumer.GetProductByIdAsync(product.Id);
            // Asserts
            Assert.NotNull(product);
            Assert.Equal(result.Name, product.Name);
            Assert.Equal(result.Description, product.Description);
            Assert.Equal(result.Price, product.Price);
            Assert.Equal(result.IsPromotion, product.IsSale);
            Assert.Equal(result.CategoryId, product.CategoryId);
            Assert.Equal(result.Thumb, product.Thumb);
            Assert.Equal(result.Id, product.Id);

        }


        [Fact(DisplayName = nameof(ProductConsumer_ShouldConsumeMessageFromBoth))]
        [Trait("Integration - Infra.Consumer", "Product Consumer")]
        public async Task ProductConsumer_ShouldConsumeMessageFromBoth()
        {
            // Arranges
            await PersistirProductCacheAndMysql();
            var products = await GetAllProductsCacheAsync();
            var product = products.FirstOrDefault();

            var result = await _productConsumer.GetProductByIdAsync(product.Id);
            // Asserts
            Assert.NotNull(product);
            Assert.Equal(result.Name, product.Name);
            Assert.Equal(result.Description, product.Description);
            Assert.Equal(result.Price, product.Price);
            Assert.Equal(result.IsPromotion, product.IsSale);
            Assert.Equal(result.CategoryId, product.CategoryId);
            Assert.Equal(result.Thumb, product.Thumb);
            Assert.Equal(result.Id, product.Id);
           

        }
    }
}
