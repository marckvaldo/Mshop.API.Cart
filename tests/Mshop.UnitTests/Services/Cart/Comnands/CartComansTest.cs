using Moq;
using Mshop.Application.Commons.DTO;
using Mshop.Application.Interface;
using Mshop.Application.Services.Cart.Commands;
using Mshop.Application.Services.Cart.Commands.Handlers;
using Mshop.Application.Services.Cart.Queries.Handlers;
using Mshop.Core.DomainObject;
using Mshop.Core.Message;
using Mshop.Core.Message.DomainEvent;
using Mshop.Infra.Consumer.DTOs;
using Mshop.Infra.Data.Interface;
using Mshop.UnitTests.Services.Cart.Commons;
using Entity = Mshop.Domain.Entity;
using GRPc = Mshop.Infra.Consumer.DTOs;

namespace Mshop.UnitTests.Services.Cart.Commands
{
    public class CartComansTest : CartServiceTestFixture
    {
        private readonly Mock<ICartRepository> _cartRepositoryMock;
        private readonly Mock<IProductConsumer> _productConsumerMock;
        private readonly Mock<ICustomerConsumer> _customerConsumerMock;
        private readonly Mock<INotification> _notificationMock;
        private readonly Mock<IDomainEventPublisher> _publishServiceMock;
        private readonly AddItemToCartHandler _addItemToCartHandler;
        private readonly RemoveItemFromCartHandler _removeItemFromCartHandler;
        private readonly RemoveQuantityFromCartHandler _removeQuantityFromCartHandler;
        private readonly ClearCartHandler _clearCartHandler;
        private readonly CheckoutHandler _checkoutHandler;
        private readonly AddCustomerHandler _addCustomerHandler;
        private readonly GetCartDetailsHandler _getCartDetailsQueryHandler;
        private readonly AddPaymentHandler _addPaymentHandler;
        public CartComansTest(): base()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _productConsumerMock = new Mock<IProductConsumer>();
            _customerConsumerMock = new Mock<ICustomerConsumer>();
            _notificationMock = new Mock<INotification>();
            _publishServiceMock = new Mock<IDomainEventPublisher>();

            _addItemToCartHandler = new AddItemToCartHandler(
                _cartRepositoryMock.Object, 
                _productConsumerMock.Object,
                _publishServiceMock.Object,
                _notificationMock.Object);

            _removeItemFromCartHandler = new RemoveItemFromCartHandler(
                _cartRepositoryMock.Object,
                _publishServiceMock.Object,
                _notificationMock.Object);

            _clearCartHandler = new ClearCartHandler(
                _cartRepositoryMock.Object,
                _publishServiceMock.Object,
                _notificationMock.Object);

            _checkoutHandler = new CheckoutHandler(
                _cartRepositoryMock.Object, 
                _publishServiceMock.Object, 
                _notificationMock.Object);

            _addCustomerHandler = new AddCustomerHandler(
                _notificationMock.Object, 
                _cartRepositoryMock.Object, 
                _customerConsumerMock.Object,
                _publishServiceMock.Object);

            _getCartDetailsQueryHandler = new GetCartDetailsHandler(
                _notificationMock.Object, 
                _cartRepositoryMock.Object);

            _addPaymentHandler = new AddPaymentHandler(
                _cartRepositoryMock.Object, 
                _publishServiceMock.Object, 
                _notificationMock.Object);

            _removeQuantityFromCartHandler = new RemoveQuantityFromCartHandler(
                _cartRepositoryMock.Object,
                _publishServiceMock.Object,
                _notificationMock.Object);

        }

        [Fact(DisplayName = nameof(AddItemToCart_ShouldReturnFalse_WhenProductNotFound))]
        [Trait("Services-Commands", "Cart")]
        public async void AddItemToCart_ShouldReturnFalse_WhenProductNotFound()
        {
            _cartRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Entity.Cart)null);


            _productConsumerMock.Setup(x => x.GetProductByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((ProductModel)null);


            var result = await _addItemToCartHandler.Handle(new AddItemToCartCommand(Guid.Empty, Guid.NewGuid(), 1), CancellationToken.None);

            Assert.False(result.IsSuccess);
            _notificationMock.Verify(x => x.AddNotifications(It.Is<string>(msg => msg == "Não foi possivel encontrar o produto")), Times.Once);
        }


        [Fact(DisplayName = nameof(AddItemToCart_ShouldAddProduct_WhenValid))]
        [Trait("Services-Commands", "Cart")]
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

            var result = await _addItemToCartHandler.Handle(new AddItemToCartCommand(cart.Id, product.Id, 1), CancellationToken.None);
            

            Assert.True(result.IsSuccess);
            _cartRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Entity.Cart>(), CancellationToken.None), Times.Once);
            _notificationMock.Verify(x => x.AddNotifications(It.IsAny<string>()), Times.Never);
            _productConsumerMock.Verify(x => x.GetProductByIdAsync(It.IsAny<Guid>()), Times.Once);

        }


        [Fact(DisplayName = nameof(RemoveItemFromCart_ShouldReturnFalse_WhenCartNotFound))]
        [Trait("Services-Commands", "Cart")]
        public async Task RemoveItemFromCart_ShouldReturnFalse_WhenCartNotFound()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var productId = Guid.NewGuid();

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId))
                .ReturnsAsync((Entity.Cart)null);

            _publishServiceMock.Setup(repo => repo.PublishAsync(It.IsAny<DomainEvent>())).ReturnsAsync(true);

            // Act
            var result = await _removeItemFromCartHandler.Handle(new RemoveItemFromCartCommand(cartId, productId), CancellationToken.None);

            // Assert
            Assert.False(result);
            _publishServiceMock.Verify(repo => repo.PublishAsync(It.IsAny<DomainEvent>()), Times.Never);
            _notificationMock.Verify(n => n.AddNotifications(It.Is<string>(msg => msg == "Não foi possivel encontrar o carrinho de compras")), Times.Once);
        }


        [Fact(DisplayName = nameof(RemoveItemFromCart_ShouldRemoveItem_WhenCartExists))]
        [Trait("Services-Commands", "Cart")]
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

            _publishServiceMock.Setup(repo => repo.PublishAsync(It.IsAny<DomainEvent>())).ReturnsAsync(true);

            // Act
            var result = await _removeItemFromCartHandler.Handle(new RemoveItemFromCartCommand(cart.Id, product.Id), CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Empty(cart.Products);
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Entity.Cart>(c => c.Products.Count == 0), CancellationToken.None), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            _publishServiceMock.Verify(repo => repo.PublishAsync(It.IsAny<IEnumerable<DomainEvent>>()), Times.Once);
        }

        [Fact(DisplayName = nameof(RemoveQuantityItemFromCart_ShouldRemoveItem_WhenCartExists))]
        [Trait("Services-Commands", "Cart")]
        public async Task RemoveQuantityItemFromCart_ShouldRemoveItem_WhenCartExists()
        {
            // Arrange
            var product = FakerProduct();
            var cart = FakerCart();
            cart.AddItem(product, 2);

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(cart);

            _cartRepositoryMock.Setup(repo => repo.UpdateAsync(cart, CancellationToken.None))
                .Returns(Task.CompletedTask);

            _publishServiceMock.Setup(repo => repo.PublishAsync(It.IsAny<DomainEvent>())).ReturnsAsync(true);

            // Act
            var result = await _removeQuantityFromCartHandler.Handle(new RemoveQuantityFromCommand(cart.Id, product.Id , 1), CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.NotEmpty(cart.Products);
            Assert.True(cart.Products.Any(p => p.Id == product.Id && p.Quantity == 1));
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Entity.Cart>(c => c.Products.Any(p=>p.Id == product.Id && p.Quantity == 1)), CancellationToken.None), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            _publishServiceMock.Verify(repo => repo.PublishAsync(It.IsAny<IEnumerable<DomainEvent>>()), Times.Once);
        }


        [Fact(DisplayName = nameof(ClearCart_ShouldClearItems_WhenCartExists))]
        [Trait("Services-Commands", "Cart")]
        public async Task ClearCart_ShouldClearItems_WhenCartExists()
        {
            // Arrange
            var cartId = Guid.NewGuid();
            var cart = FakerCart(true);


            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cartId))
                .ReturnsAsync(cart);

            _cartRepositoryMock.Setup(repo => repo.UpdateAsync(cart, CancellationToken.None))
                .Returns(Task.CompletedTask);

            _publishServiceMock.Setup(repo => repo.PublishAsync(It.IsAny<DomainEvent>())).ReturnsAsync(true);

            // Act
            var result = await _clearCartHandler.Handle(new ClearCartCommand(cartId), CancellationToken.None);

            // Assert
            Assert.True(result);
            Assert.Empty(cart.Products);
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Entity.Cart>(c => !c.Products.Any()), CancellationToken.None), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            _publishServiceMock.Verify(repo => repo.PublishAsync(It.IsAny<IEnumerable<DomainEvent>>()), Times.Once);

        }


        [Fact(DisplayName = nameof(Checkout_ShouldReturnTrue_WhenCartAndCustomerAreValid))]
        [Trait("Services-Commands", "Cart")]
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

            _publishServiceMock.Setup(repo => repo.PublishAsync(It.IsAny<DomainEvent>())).ReturnsAsync(true);

            // Act
            var result = await _addCustomerHandler.Handle(new AddCustomerToCartCommand(cart.Id, customer.Id), CancellationToken.None);

            // Assert
            Assert.True(result);
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.Is<Entity.Cart>(c => c.Customer != null), CancellationToken.None), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            _publishServiceMock.Verify(repo => repo.PublishAsync(It.IsAny<IEnumerable<DomainEvent>>()), Times.Once);
        }


        [Fact(DisplayName = nameof(AddItemToCart_ShouldCreateNewCart_WhenCartDoesNotExist))]
        [Trait("Services-Commands", "Cart")]
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

            _publishServiceMock.Setup(repo => repo.PublishAsync(It.IsAny<DomainEvent>())).ReturnsAsync(true);

            // Act
            var result = await _addItemToCartHandler.Handle(new AddItemToCartCommand(Guid.Empty, product.Id, 1), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _cartRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Entity.Cart>(), CancellationToken.None), Times.Once);
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Entity.Cart>(), CancellationToken.None), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            _publishServiceMock.Verify(repo => repo.PublishAsync(It.IsAny<IEnumerable<DomainEvent>>()), Times.Once);
        }


        [Fact(DisplayName = nameof(AddPaymentCart_ShouldReturnTrue))]
        [Trait("Services-Commands", "Cart")]
        public async Task AddPaymentCart_ShouldReturnTrue()
        {
            // Arrange
            var cart = FakerCart(true,true,false);
            var payment = FakerPayment(Entity.PaymentMethod.CreditCard,Entity.PaymentStatus.Pending,cart.GetTotal());

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cart.Id))
                .ReturnsAsync(cart);

            _publishServiceMock.Setup(repo => repo.PublishAsync(It.IsAny<DomainEvent>())).ReturnsAsync(true);

            // Act
            var result = await _addPaymentHandler.Handle(new AddPaymentToCartCommand(
                cart.Id, 
                new PaymentDTO(
                    payment.Amount,
                    payment.PaymentMethod,
                    payment.Status,
                    payment.Installments,
                    payment.CardToken,
                    payment.BoletoNumber,
                    payment.BoletoDueDate)),CancellationToken.None); //_cartServices.AddPayment(cart.Id, payment);

            // Assert
            Assert.True(result);
            _cartRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Entity.Cart>(), CancellationToken.None), Times.Once);
            _cartRepositoryMock.Verify(r=>r.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
            _notificationMock.Verify(n => n.AddNotifications(It.IsAny<string>()), Times.Never);
            _publishServiceMock.Verify(repo => repo.PublishAsync(It.IsAny<IEnumerable<DomainEvent>>()), Times.Once);
            //_notificationMock.Verify(n => n.AddNotifications(It.Is<string>(msg => msg == "Não foi possivel encontrar o carrinho de compras")), Times.Once);
        }

    }
}
