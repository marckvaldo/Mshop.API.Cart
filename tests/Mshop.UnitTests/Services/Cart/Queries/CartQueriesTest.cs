using Moq;
using Mshop.Application.Interface;
using Mshop.Application.Services.Cart.Commands.Handlers;
using Mshop.Application.Services.Cart.Queries;
using Mshop.Application.Services.Cart.Queries.Handlers;
using Mshop.Core.Message;
using Mshop.Core.Message.DomainEvent;
using Mshop.Infra.Data.Interface;
using Mshop.UnitTests.Services.Cart.Commons;

namespace Mshop.UnitTests.Services.Cart.Queries
{
    public class CartQueriesTest : CartServiceTestFixture
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
        public CartQueriesTest(): base()
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

        [Fact(DisplayName = nameof(GetCartDetails_ShouldReturnCartDTO_WhenCartExists))]
        [Trait("Services-Queries", "Cart")]
        public async Task GetCartDetails_ShouldReturnCartDTO_WhenCartExists()
        {
            // Arrange            
            var cart = FakerCart(true,true);

            _cartRepositoryMock.Setup(repo => repo.GetByIdAsync(cart.Id))
                .ReturnsAsync(cart);

            // Act
            var result = await _getCartDetailsQueryHandler.Handle(new GetCartDetailsQuery(cart.Id), CancellationToken.None);  //_cartServices.GetCartDetails(cart.Id);

            // Assert
            Assert.NotNull(result.Data);
            Assert.Equal(cart.Id, result.Data.Id);
            Assert.NotNull(result.Data.Customer);
            Assert.Equal(cart.Customer.Id, result.Data.Customer.Id);
            Assert.Equal(cart.Customer.Name, result.Data.Customer.Name);
            Assert.Equal(cart.Customer.Email, result.Data.Customer.Email);
            Assert.Equal(cart.Customer.Phone, result.Data.Customer.Phone);
            Assert.Equal(cart.Products.Count(), result.Data.Products.Count());

        }


    }
}
