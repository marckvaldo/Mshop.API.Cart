using Moq;
using Mshop.Application.Services.Cart;
using Mshop.Core.Message;
using GRPc = Mshop.Infra.Consumer.DTOs;
using Mshop.Infra.Data.Interface;
using Entity = Mshop.Domain.Entity;
using Mshop.Infra.Consumer.DTOs;
using Mshop.Application.Interface;
using Mshop.Core.DomainObject;
using Mshop.Core.Message.DomainEvent;
using Bogus;

namespace Mshop.UnitTests.Services
{
    public class CartServiceTest : CartServiceTestFixture
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<IProductConsumer> _productConsumerMock;
        private readonly Mock<ICustomerConsumer> _customerConsumerMock;
        private readonly Mock<INotification> _notificationMock;
        private readonly Mock<IDomainEventPublisher> _publishServiceMock;
        private readonly CartServices _cartServices;
        public CartServiceTest(): base()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _productConsumerMock = new Mock<IProductConsumer>();
            _customerConsumerMock = new Mock<ICustomerConsumer>();
            _notificationMock = new Mock<INotification>();
            _publishServiceMock = new Mock<IDomainEventPublisher>();

            _cartServices = new CartServices(
                _cartRepositoryMock.Object,
                _productConsumerMock.Object,
                _customerConsumerMock.Object,
                _publishServiceMock.Object,
                _notificationMock.Object
                );

        }

        [Fact(DisplayName = nameof(AddItemToCart_ShouldReturnFalse_WhenProductNotFound))]
        [Trait("Services", "Cart")]
        public async void AddItemToCart_ShouldReturnFalse_WhenProductNotFound()
        {
            _cartRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Entity.Cart)null);


            _productConsumerMock.Setup(x => x.GetProductByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((ProductModel)null);


            var result = await _cartServices.AddItemToCart(Guid.Empty, Guid.NewGuid(), 1);

            Assert.False(result.IsSuccess);
            _notificationMock.Verify(x => x.AddNotifications(It.Is<string>(msg => msg == "Não foi possivel encontrar o produto")), Times.Once);
        }


        [Fact(DisplayName = nameof(AddItemToCart_ShouldAddProduct_WhenValid))]
        [Trait("Services", "Cart")]
        public async Task AddItemToCart_ShouldAddProduct_WhenValid()
        {
            var cart = FakerCart();
            var product = FakerProduct();

            _cartRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(cart);

            _productConsumerMock.Setup(x => x.GetProductByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new GRPc.ProductModel(
                product.Id,
                product.Description,
                product.Name,
                product.Price,
                product.IsSale,
                product.CategoryId,
                product.Category,
                product.Thumb));

            _cartRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Entity.Cart>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            var result = await _cartServices.AddItemToCart(cart.Id, product.Id, 1);

            Assert.True(result.IsSuccess);
            _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Entity.Cart>(), CancellationToken.None), Times.Once);
            _notificationMock.Verify(x => x.AddNotifications(It.IsAny<string>()), Times.Never);
            _productConsumerMock.Verify(x => x.GetProductByIdAsync(It.IsAny<Guid>()), Times.Once);

        }


        [Fact(DisplayName = nameof(RemoveItemFromCart_ShouldReturnFalse_WhenCartNotFound))]
        [Trait("Services", "Cart")]
        public async Task RemoveItemFromCart_ShouldReturnFalse_WhenCartNotFound()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId))
                .ReturnsAsync((Entity.Cart)null);

            // Act
            var result = await _cartServices.RemoveItemFromCart(cartId, productId);

            // Assert
            Assert.False(result);
            _notificationMock.Verify(n => n.AddNotifications(It.Is<string>(msg => msg == "Não foi possivel encontrar o carrinho de compras")), Times.Once);
        }


        [Fact(DisplayName = nameof(RemoveItemFromCart_ShouldRemoveItem_WhenCartExists))]
        [Trait("Services", "Cart")]
        public async Task RemoveItemFromCart_ShouldRemoveItem_WhenCartExists()
        {
            // Arrange
            var product = FakerProduct();
            var cart = FakerCart();
            cart.AddItem(product, 1);

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(cart);

            _cartRepositoryMock.Setup(repo => repo.UpdateAsync(cart, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartServices.RemoveItemFromCart(cart.Id, product.Id);

            // Assert
            Assert.True(result);
            Assert.Empty(cart.Products);
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Entity.Cart>(c => c.Products.Count == 0), CancellationToken.None), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
        }


        [Fact(DisplayName = nameof(ClearCart_ShouldClearItems_WhenCartExists))]
        [Trait("Services", "Cart")]
        public async Task ClearCart_ShouldClearItems_WhenCartExists()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cart = FakerCart(true);


            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId))
                .ReturnsAsync(cart);

            _cartRepositoryMock.Setup(repo => repo.UpdateAsync(cart, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartServices.ClearCart(cartId);

            // Assert
            Assert.True(result);
            Assert.Empty(cart.Products);
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Entity.Cart>(c => !c.Products.Any()), CancellationToken.None), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
           
        }


        [Fact(DisplayName = nameof(Checkout_ShouldReturnTrue_WhenCartAndCustomerAreValid))]
        [Trait("Services", "Cart")]
        public async Task Checkout_ShouldReturnTrue_WhenCartAndCustomerAreValid()
        {
            // Arrange
            var customer = FakerCustomer();
            var cart = FakerCart();

            var customerDto = new GRPc.CustomerModel(customer.Id, customer.Name, customer.Email, customer.Phone, null);

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cart.Id))
                .ReturnsAsync(cart);

            _customerConsumerMock.Setup(client => client.GetCustomerByIdAsync(customer.Id))
                .ReturnsAsync(customerDto);

            _cartRepositoryMock.Setup(repo => repo.UpdateAsync(cart, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartServices.AddCustomer(cart.Id, customer.Id);

            // Assert
            Assert.True(result);
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Entity.Cart>(c => c.Customer != null), CancellationToken.None), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
        }



        [Fact(DisplayName = nameof(AddItemToCart_ShouldCreateNewCart_WhenCartDoesNotExist))]
        [Trait("Services", "Cart")]
        public async Task AddItemToCart_ShouldCreateNewCart_WhenCartDoesNotExist()
        {
            // Arrange
            var product = FakerProduct();

            var productDto = new GRPc.ProductModel(
                product.Id,
                product.Description,
                product.Name,
                product.Price,
                product.IsSale,
                product.CategoryId,
                product.Category,
                product.Thumb);

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Entity.Cart)null);

            _productConsumerMock.Setup(client => client.GetProductByIdAsync(product.Id))
                .ReturnsAsync(productDto);

            _cartRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Entity.Cart>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            _cartRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Entity.Cart>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cartServices.AddItemToCart(Guid.Empty, product.Id, 1);

            // Assert
            Assert.True(result.IsSuccess);
            _cartRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Entity.Cart>(), CancellationToken.None), Times.Once);
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Entity.Cart>(), CancellationToken.None), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
        }


        [Fact(DisplayName = nameof(GetCartDetails_ShouldReturnCartDTO_WhenCartExists))]
        [Trait("Services", "Cart")]
        public async Task GetCartDetails_ShouldReturnCartDTO_WhenCartExists()
        {
            // Arrange            
            var cart = FakerCart(true,true);

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cart.Id))
                .ReturnsAsync(cart);

            // Act
            var result = await _cartServices.GetCartDetails(cart.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cart.Id, result.Id);
            Assert.NotNull(result.Customer);
            Assert.Equal(cart.Customer.Id, result.Customer.Id);
            Assert.Equal(cart.Customer.Name, result.Customer.Name);
            Assert.Equal(cart.Customer.Email, result.Customer.Email);
            Assert.Equal(cart.Customer.Phone, result.Customer.Phone);
            Assert.Equal(cart.Products.Count, result.Products.Count);

        }


        [Fact(DisplayName = nameof(AddPaymentCart_ShouldReturnTrue))]
        [Trait("Services", "Cart")]
        public async Task AddPaymentCart_ShouldReturnTrue()
        {
            // Arrange
            var cart = FakerCart(true,true,false);
            var payment = FakerPayment(Entity.PaymentMethod.CreditCard,Entity.PaymentStatus.Pending,cart.GetTotal());

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cart.Id))
                .ReturnsAsync(cart);

            // Act
            var result = await _cartServices.AddPayment(cart.Id, payment);

            // Assert
            Assert.True(result);
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Entity.Cart>(), CancellationToken.None), Times.Once);
            _cartRepositoryMock.Verify(r=>r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            //_notificationMock.Verify(n => n.AddNotifications(It.Is<string>(msg => msg == "Não foi possivel encontrar o carrinho de compras")), Times.Once);
        }

    }
}
