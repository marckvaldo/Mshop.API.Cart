using Mshop.Core.Message;
using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.UnitTests.Domain.Entity.Products
{
    public class ProductTest : ProductTestFixture
    {
        public ProductTest() : base()
        {
            
        }

        [Fact(DisplayName = nameof(Product_ShouldInitializeCorrectly))]
        [Trait("Domain", "Products")]
        public void Product_ShouldInitializeCorrectly()
        {
            var productFake = FakerProduct();
            var notification = new Notifications();
            // Act
            var product = new Product(
                productFake.Id,
                productFake.Description,
                productFake.Name,
                productFake.Price,
                productFake.IsSale, 
                productFake.CategoryId,
                productFake.Category,
                productFake.Thumb,
                productFake.Quantity
                );

            var IsValid = product.IsValid(notification);

            // Assert
            Assert.True(IsValid);
            Assert.Equal(productFake.Description, product.Description);
            Assert.Equal(productFake.Name, product.Name);
            Assert.Equal(productFake.Price, product.Price);
            Assert.Equal(productFake.IsSale, product.IsSale);
            Assert.Equal(productFake.CategoryId, product.CategoryId);
            Assert.Equal(productFake.Category, product.Category);
            Assert.Equal(productFake.Thumb, product.Thumb);
            Assert.Equal(productFake.Quantity, product.Quantity); // Quantity starts at 0
            Assert.Equal(productFake.Total, product.Total);  // Total starts at 0

        }

        [Fact(DisplayName = nameof(UpdateQuantity_ShouldSetQuantityAndRecalculateTotal))]
        [Trait("Domain", "Products")]
        public void UpdateQuantity_ShouldSetQuantityAndRecalculateTotal()
        {
            // Arrange
            var product = FakerProduct();
            var notification = new Notifications();

            // Act
            product.UpdateQuantity(5);
            var IsValid = product.IsValid(notification);

            // Assert
            Assert.True(IsValid);
            Assert.Equal(5, product.Quantity);
            Assert.Equal(product.Price * 5, product.Total); // 10.50 * 5
        }

        [Fact(DisplayName = nameof(GetTotal_ShouldReturnCorrectTotal))]
        [Trait("Domain", "Products")]
        public void GetTotal_ShouldReturnCorrectTotal()
        {
            // Arrange
            var notification = new Notifications();
            var product = FakerProduct();

            product.UpdateQuantity(3);
            var Errors = product.IsValid(notification);

            // Act
            var total = product.GetTotal();

            // Assert
            Assert.True(Errors);
            Assert.Equal(product.Price * product.Quantity, total); // 20 * 3
        }

        [Fact(DisplayName = nameof(UpdateQuantity_ShouldAllowZeroQuantity))]
        [Trait("Domain", "Products")]
        public void UpdateQuantity_ShouldAllowZeroQuantity()
        {
            // Arrange
            var notification = new Notifications();
            var product = FakerProduct();

            // Act
            product.UpdateQuantity(0);
            var IsValid = product.IsValid(notification);

            // Assert
            Assert.False(IsValid);
            Assert.Equal(0, product.Quantity);
            Assert.Equal(0m, product.Total); // 15 * 0
        }

        [Fact(DisplayName = nameof(UpdateQuantity_ShouldAllowNegativeQuantity))]
        [Trait("Domain", "Products")]
        public void UpdateQuantity_ShouldAllowNegativeQuantity()
        {
            // Arrange
            var notification = new Notifications();
            var product = FakerProduct();

            // Act
            product.UpdateQuantity(-5);
            var IsValid = product.IsValid(notification);

            // Assert
            Assert.False(IsValid);
        }

    }
}
