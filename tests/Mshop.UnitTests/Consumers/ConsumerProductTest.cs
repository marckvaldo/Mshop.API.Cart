using Moq;
using Mshop.Application.Consumers;
using Mshop.Core.Message;
using Mshop.Infra.Consumer.Cache;
//using Mshop.Infra.Consumer.Clients;
using Mshop.Infra.Consumer.DTOs;
using Mshop.Infra.Consumer.GRPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.UnitTests.Consumers
{
    public class ConsumerProductTest : ConsumerProductTestFixture
    {

        private readonly Mock<IServiceCache> _mockServiceCache;
        private readonly Mock<IServicerGRPC> _mockServicerGRPC;
        private readonly Mock<INotification> _mockNotification;
        private readonly ProductConsumer _productConsumer;

        public ConsumerProductTest() :base()
        {
            _mockServiceCache = new Mock<IServiceCache>();
            _mockServicerGRPC = new Mock<IServicerGRPC>();
            _mockNotification = new Mock<INotification>();
            _productConsumer = new ProductConsumer(_mockServiceCache.Object, _mockServicerGRPC.Object, _mockNotification.Object);
        }


        [Fact(DisplayName = nameof(GetProductByIdAsync_ShouldReturnNull_WhenProductIdIsEmpty))]
        [Trait("Consumer", "Product")]
        public async Task GetProductByIdAsync_ShouldReturnNull_WhenProductIdIsEmpty()
        {
            // Arrange
            var emptyProductId = Guid.Empty;

            // Act
            var result = await _productConsumer.GetProductByIdAsync(emptyProductId);

            // Assert
            Assert.Null(result);
            _mockNotification.Verify(n => n.AddNotifications(It.Is<string>(s => s == "ProductId cannot be empty")), Times.Once);
        }

        [Fact(DisplayName = nameof(GetProductByIdAsync_ShouldReturnProduct_FromGrpcService))]
        [Trait("Consumer", "Product")]
        public async Task GetProductByIdAsync_ShouldReturnProduct_FromGrpcService()
        {
            // Arrange
            var product = FakerProduct();
            var expectedProduct = new ProductModel(
                product.Id,
                product.Description, 
                product.Name, 
                product.Price,
                product.IsSale,
                product.CategoryId, 
                product.Category, 
                product.Thumb);

            _mockServicerGRPC
                .Setup(s => s.GetProductByIdAsync(product.Id))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _productConsumer.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct, result);
            _mockNotification.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            _mockServiceCache.Verify(c => c.GetProductById(It.IsAny<Guid>()), Times.Never);
        }

        [Fact(DisplayName = nameof(GetProductByIdAsync_ShouldReturnProduct_FromCache_WhenGrpcServiceReturnsNull))]
        [Trait("Consumer", "Product")]
        public async Task GetProductByIdAsync_ShouldReturnProduct_FromCache_WhenGrpcServiceReturnsNull()
        {
            // Arrange
            var product = FakerProduct();
            var expectedProduct = new ProductModel(
                product.Id,
                product.Description,
                product.Name,
                product.Price,
                product.IsSale,
                product.CategoryId,
                product.Category,
                product.Thumb);


            _mockServicerGRPC
                .Setup(s => s.GetProductByIdAsync(product.Id))
                .ReturnsAsync((ProductModel?)null);

            _mockServiceCache
                .Setup(c => c.GetProductById(product.Id))
                .ReturnsAsync(expectedProduct);

            // Act
            var result = await _productConsumer.GetProductByIdAsync(product.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProduct, result);
            _mockNotification.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = nameof(GetProductByIdAsync_ShouldAddNotification_WhenProductNotFound))]
        [Trait("Consumer", "Product")]
        public async Task GetProductByIdAsync_ShouldAddNotification_WhenProductNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();

            _mockServicerGRPC
                .Setup(s => s.GetProductByIdAsync(productId))
                .ReturnsAsync((ProductModel?)null);

            _mockServiceCache
                .Setup(c => c.GetProductById(productId))
                .ReturnsAsync((ProductModel?)null);

            // Act
            var result = await _productConsumer.GetProductByIdAsync(productId);

            // Assert
            Assert.Null(result);
            _mockNotification.Verify(n => n.AddNotifications(It.Is<string>(s => s == "Product not found")), Times.Once);
        }
    }
}
