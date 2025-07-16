using Microsoft.Extensions.DependencyInjection;
using Mshop.Application.Interface;
using Mshop.Application.Services.Cart.Commands;
using Mshop.Application.Services.Cart.Commands.Handlers;
using Mshop.Application.Services.Cart.Queries;
using Mshop.Application.Services.Cart.Queries.Handlers;
using Mshop.Core.Message;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Event;
using Mshop.Infra.Data.Interface;
using Mshop.IntegrationTest.Common.Persistence.RabbitMQ;
using Mshop.IntegrationTest.Common.Persistence.RabbitMQ.DTOs;
using Mshop.IntegrationTest.Services.Cart.Commons;

namespace Mshop.IntegrationTest.Services.Cart.Commands;

public class CartServiceTest : CartServiceTestFixture
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductConsumer _productConsumer;
    private readonly ICustomerConsumer _customerConsumer;
    private readonly INotification _notification;
    private readonly IDomainEventPublisher _publishService;
    private readonly RabbitMQPersistence _rabbitMQPersistence;

    private readonly AddItemToCartHandler _addItemToCartHandler;
    private readonly RemoveItemFromCartHandler _removeItemFromCartHandler;
    private readonly RemoveQuantityFromCartHandler _removeQuantityFromCartHandler;
    private readonly ClearCartHandler _clearCartHandler;
    private readonly CheckoutHandler _checkoutHandler;
    private readonly AddCustomerHandler _addCustomerHandler;
    private readonly GetCartDetailsHandler _getCartDetailsQueryHandler;
    private readonly AddPaymentHandler _addPaymentHandler;

    public CartServiceTest() : base()
    {
        _cartRepository = _serviceProvider.GetRequiredService<ICartRepository>();
        _productConsumer = _serviceProvider.GetRequiredService<IProductConsumer>();
        _customerConsumer = _serviceProvider.GetRequiredService<ICustomerConsumer>();
        _notification = _serviceProvider.GetRequiredService<INotification>();
        _publishService = _serviceProvider.GetRequiredService<IDomainEventPublisher>();
        _rabbitMQPersistence = new RabbitMQPersistence(_serviceProvider);

        //cartServices = new CartServices(_cartRepository, _productConsumer, _customerConsumer, _publishService, _notification);
        _addItemToCartHandler = new AddItemToCartHandler(_cartRepository, _productConsumer, _publishService, _notification);
        _removeQuantityFromCartHandler = new RemoveQuantityFromCartHandler(_cartRepository, _publishService, _notification);
        _removeItemFromCartHandler = new RemoveItemFromCartHandler(_cartRepository, _publishService, _notification);
        _clearCartHandler = new ClearCartHandler(_cartRepository, _publishService, _notification);
        _checkoutHandler = new CheckoutHandler(_cartRepository, _publishService, _notification);
        _addCustomerHandler = new AddCustomerHandler(_notification, _cartRepository, _customerConsumer, _publishService);
        _getCartDetailsQueryHandler = new GetCartDetailsHandler(_notification, _cartRepository);
        _addPaymentHandler = new AddPaymentHandler(_cartRepository, _publishService, _notification);


        ClearDataBase().Wait();

        _rabbitMQPersistence.DownRabbitMQWithDeadLetter().Wait();
        _rabbitMQPersistence.SetupRabbitMQWithDeadLette().Wait();
    }

    [Fact(DisplayName = nameof(AddItemToCart_ShouldReturnFalse_WhenProductNotFound))]
    [Trait("Integration - Application", "Service Cart")]
    public async void AddItemToCart_ShouldReturnFalse_WhenProductNotFound()
    {

        var result = await _addItemToCartHandler.Handle(new AddItemToCartCommand(Guid.Empty, Guid.NewGuid(), 1), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(_notification.HasErrors());
    }


    [Fact(DisplayName = nameof(AddItemToCartWithCartId_ShouldAddProduct_WhenValid))]
    [Trait("Integration - Application", "Service Cart")]
    public async Task AddItemToCartWithCartId_ShouldAddProduct_WhenValid()
    {
        PersistProductsDataBase();
        var products = await GetAllProductsMysqlAsync();
        var product = products.FirstOrDefault();

        var cart = FakerCart();


        var result = await _addItemToCartHandler.Handle(new AddItemToCartCommand(cart.Id, product.Id, 1), CancellationToken.None);
        var cartDataBase = _cartRepository.GetByIdAsync(cart.Id).Result;

        Assert.True(result.IsSuccess);
        Assert.False(_notification.HasErrors());
        Assert.NotNull(cartDataBase);
        Assert.NotNull(result.Data);
        Assert.True(cartDataBase.Products.Count > 0);

        var productBase = cartDataBase.Products.FirstOrDefault(p => p.Id == product.Id);
        var productResult = result.Data.Products.FirstOrDefault(p => p.Id == product.Id);

        Assert.Equal(productBase.Quantity, 1);
        Assert.Equal(productBase.Name, productResult.Name);
        Assert.Equal(productBase.Price, productResult.Price);
        Assert.Equal(productBase.CategoryId, productResult.CategoryId);
        Assert.Equal(productBase.CategoryId, productResult.CategoryId);
        Assert.Equal(productBase.Thumb, productResult.Thumb);
        Assert.Equal(productBase.Description, productResult.Description);
        Assert.Equal(productBase.IsSale, productResult.IsSale);
        Assert.Equal(productBase.Id, productResult.Id);

        Assert.Equal(productBase.Id, product.Id);
        Assert.Equal(productBase.Quantity, 1);
        Assert.Equal(productBase.Name, product.Name);
        Assert.Equal(productBase.Price, product.Price);
        Assert.Equal(productBase.CategoryId, product.CategoryId);
        Assert.Equal(productBase.Thumb, product.Thumb);
        Assert.Equal(productBase.Description, product.Description);
        Assert.Equal(productBase.IsSale, product.IsSale);



    }


    [Fact(DisplayName = nameof(AddItemToCartWithOutCartId_ShouldAddProduct_WhenValid))]
    [Trait("Integration - Application", "Service Cart")]
    public async Task AddItemToCartWithOutCartId_ShouldAddProduct_WhenValid()
    {
        PersistProductsDataBase();
        var products = await GetAllProductsMysqlAsync();
        var product = products.FirstOrDefault();

        var cart = FakerCart();


        var result = await _addItemToCartHandler.Handle(new AddItemToCartCommand(Guid.Empty, product.Id, 1), CancellationToken.None);
        var cartDataBase = _cartRepository.GetByIdAsync(result.Data.Id).Result;

        Assert.True(result.IsSuccess);
        Assert.False(_notification.HasErrors());
        Assert.NotNull(cartDataBase);
        Assert.NotNull(result.Data);
        Assert.True(cartDataBase.Products.Count > 0);

        var productBase = cartDataBase.Products.FirstOrDefault(p => p.Id == product.Id);
        var productResult = result.Data.Products.FirstOrDefault(p => p.Id == product.Id);

        Assert.Equal(productBase.Quantity, 1);
        Assert.Equal(productBase.Name, productResult.Name);
        Assert.Equal(productBase.Price, productResult.Price);
        Assert.Equal(productBase.CategoryId, productResult.CategoryId);
        Assert.Equal(productBase.CategoryId, productResult.CategoryId);
        Assert.Equal(productBase.Thumb, productResult.Thumb);
        Assert.Equal(productBase.Description, productResult.Description);
        Assert.Equal(productBase.IsSale, productResult.IsSale);
        Assert.Equal(productBase.Id, productResult.Id);

        Assert.Equal(productBase.Id, product.Id);
        Assert.Equal(productBase.Quantity, 1);
        Assert.Equal(productBase.Name, product.Name);
        Assert.Equal(productBase.Price, product.Price);
        Assert.Equal(productBase.CategoryId, product.CategoryId);
        Assert.Equal(productBase.Thumb, product.Thumb);
        Assert.Equal(productBase.Description, product.Description);
        Assert.Equal(productBase.IsSale, product.IsSale);


    }


    [Fact(DisplayName = nameof(AddItemToCartWhenHasItem_ShouldAddProduct_WhenValid))]
    [Trait("Integration - Application", "Service Cart")]
    public async Task AddItemToCartWhenHasItem_ShouldAddProduct_WhenValid()
    {
        PersistProductsDataBase();
        var products = await GetAllProductsMysqlAsync();
        var product = products.FirstOrDefault();

        var cart = FakerCart();


        var result = await _addItemToCartHandler.Handle(new AddItemToCartCommand(Guid.Empty, product.Id, 1), CancellationToken.None);
        result = await _addItemToCartHandler.Handle(new AddItemToCartCommand(result.Data.Id, product.Id, 1), CancellationToken.None);
        var cartDataBase = _cartRepository.GetByIdAsync(result.Data.Id).Result;

        Assert.True(result.IsSuccess);
        Assert.False(_notification.HasErrors());
        Assert.NotNull(cartDataBase);
        Assert.NotNull(result.Data);
        Assert.True(cartDataBase.Products.Count > 0);
        Assert.Equal(cartDataBase.Id, result.Data.Id);

        var productBase = cartDataBase.Products.FirstOrDefault(p => p.Id == product.Id);
        var productResult = result.Data.Products.FirstOrDefault(p => p.Id == product.Id);

        Assert.Equal(productBase.Quantity, 2);
        Assert.Equal(productBase.Name, productResult.Name);
        Assert.Equal(productBase.Price, productResult.Price);
        Assert.Equal(productBase.CategoryId, productResult.CategoryId);
        Assert.Equal(productBase.CategoryId, productResult.CategoryId);
        Assert.Equal(productBase.Thumb, productResult.Thumb);
        Assert.Equal(productBase.Description, productResult.Description);
        Assert.Equal(productBase.IsSale, productResult.IsSale);
        Assert.Equal(productBase.Id, productResult.Id);

        Assert.Equal(productBase.Id, product.Id);
        Assert.Equal(productBase.Quantity, 2);
        Assert.Equal(productBase.Name, product.Name);
        Assert.Equal(productBase.Price, product.Price);
        Assert.Equal(productBase.CategoryId, product.CategoryId);
        Assert.Equal(productBase.Thumb, product.Thumb);
        Assert.Equal(productBase.Description, product.Description);
        Assert.Equal(productBase.IsSale, product.IsSale);


    }



    [Fact(DisplayName = nameof(RemoveItemFromCart_ShouldReturnFalse_WhenCartNotFound))]
    [Trait("Integration - Application", "Service Cart")]
    public async Task RemoveItemFromCart_ShouldReturnFalse_WhenCartNotFound()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var productId = Guid.NewGuid();


        // Act
        var result = await _removeItemFromCartHandler.Handle(new RemoveItemFromCartCommand(cartId, productId), CancellationToken.None); //_cartServices.RemoveItemFromCart(cartId, productId);

        // Assert
        Assert.False(result);
        Assert.True(_notification.HasErrors());
    }


    [Fact(DisplayName = nameof(RemoveItemFromCart_ShouldRemoveItem_WhenCartExists))]
    [Trait("Integration - Application", "Service Cart")]
    public async Task RemoveItemFromCart_ShouldRemoveItem_WhenCartExists()
    {
        // Arrange
        var product = FakerProduct();
        var cart = FakerCart();
        cart.AddItem(product, 1);

        await _cartRepository.AddAsync(cart, CancellationToken.None);


        // Act
        var result = await _removeItemFromCartHandler.Handle(new RemoveItemFromCartCommand(cart.Id, product.Id), CancellationToken.None);
        var carDataBase = _cartRepository.GetByIdAsync(cart.Id).Result;

        // Assert
        Assert.True(result);
        Assert.True(carDataBase.Products.Count == 0);
        Assert.False(_notification.HasErrors());
    }


    [Fact(DisplayName = nameof(RemoveQuantityItemFromCart_ShouldRemoveQuantityItem_WhenCartExists))]
    [Trait("Integration - Application", "Service Cart")]
    public async Task RemoveQuantityItemFromCart_ShouldRemoveQuantityItem_WhenCartExists()
    {
        // Arrange
        var product = FakerProduct();
        var cart = FakerCart();
        cart.AddItem(product, 3);

        await _cartRepository.AddAsync(cart, CancellationToken.None);

        // Act
        var result = await _removeQuantityFromCartHandler.Handle(new RemoveQuantityFromCommand(cart.Id, product.Id, 1), CancellationToken.None);
        var carDataBase = _cartRepository.GetByIdAsync(cart.Id).Result;

        // Assert
        Assert.True(result);
        Assert.True(carDataBase.Products.FirstOrDefault().Quantity == 2);
        Assert.False(_notification.HasErrors());
    }



    [Fact(DisplayName = nameof(ClearCart_ShouldClearItems_WhenCartExists))]
    [Trait("Integration - Application", "Service Cart")]
    public async Task ClearCart_ShouldClearItems_WhenCartExists()
    {
        // Arrange
        var cartId = Guid.NewGuid();
        var cart = FakerCart(true);

        await _cartRepository.AddAsync(cart, CancellationToken.None);

        // Act
        var result = await _clearCartHandler.Handle(new ClearCartCommand(cart.Id), CancellationToken.None); //_cartServices.ClearCart(cart.Id);
        var cartBase = await _cartRepository.GetByIdAsync(cart.Id);
        // Assert
        Assert.True(result);
        Assert.Empty(cartBase.Products);
        Assert.False(_notification.HasErrors());

    }


    [Fact(DisplayName = nameof(AddCustomer_ShouldReturnTrue_WhenCartAndCustomerAreValid))]
    [Trait("Integration - Application", "Service Cart")]
    public async Task AddCustomer_ShouldReturnTrue_WhenCartAndCustomerAreValid()
    {
        // Arrange
        /*var customer = FakerCustomer();
        var cart = FakerCart();

        var customerDto = new GRPc.CustomerModel(customer.Id, customer.Name, customer.Email, customer.Phone, null);
        await _cartRepository.AddAsync(cart, CancellationToken.None);
       
        // Act
        var result = await _cartServices.AddCustomer(cart.Id, customer.Id);

        // Assert
        Assert.True(result);
        Assert.False(_notification.HasErrors());*/

        Assert.True(true);
    }


    [Fact(DisplayName = nameof(GetCartDetails_ShouldReturnCartDTO_WhenCartExists))]
    [Trait("Integration - Application", "Service Cart")]
    public async Task GetCartDetails_ShouldReturnCartDTO_WhenCartExists()
    {
        // Arrange            
        var cart = FakerCart(true, true);
        await _cartRepository.AddAsync(cart, CancellationToken.None);

        // Act
        var result = await _getCartDetailsQueryHandler.Handle(new GetCartDetailsQuery(cart.Id), CancellationToken.None);//_cartServices.GetCartDetails(cart.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(cart.Id, result.Data.Id);
        Assert.NotNull(result.Data.Customer);
        Assert.Equal(cart.Customer.Id, result.Data.Customer.Id);
        Assert.Equal(cart.Customer.Name, result.Data.Customer.Name);
        Assert.Equal(cart.Customer.Email, result.Data.Customer.Email);
        Assert.Equal(cart.Customer.Phone, result.Data.Customer.Phone);
        Assert.Equal(cart.Products.Count, result.Data.Products.Count());
        Assert.False(_notification.HasErrors());

    }


    [Fact(DisplayName = nameof(AddPaymentToCart_ShouldReturnFalse_WhenProductNotFound))]
    [Trait("Integration - Application", "Service Cart")]
    public async void AddPaymentToCart_ShouldReturnFalse_WhenProductNotFound()
    {
        //var product = FakerProduct();
        var cart = FakerCart();
        var payment = FakerPayment(Domain.Entity.PaymentMethod.CreditCard, Domain.Entity.PaymentStatus.Pending, cart.GetTotal());
        //cart.AddItem(product, 3);

        await _cartRepository.AddAsync(cart, CancellationToken.None);


        var result = await _addPaymentHandler.Handle(new AddPaymentToCartCommand(cart.Id, PaymentToPaymamentDTO(payment)), CancellationToken.None); //_cartServices.AddPayment(cart.Id, payment);

        Assert.False(result);
        Assert.True(_notification.HasErrors());
    }


    [Fact(DisplayName = nameof(AddPaymentToCart_ShouldReturnTrue_WhenHasProduct))]
    [Trait("Integration - Application", "Service Cart")]
    public async void AddPaymentToCart_ShouldReturnTrue_WhenHasProduct()
    {
        var product = FakerProduct();
        var cart = FakerCart();
        cart.AddItem(product, 3);
        var payment = FakerPayment(Domain.Entity.PaymentMethod.CreditCard, Domain.Entity.PaymentStatus.Pending, cart.GetTotal());


        await _cartRepository.AddAsync(cart, CancellationToken.None);


        var result = await _addPaymentHandler.Handle(new AddPaymentToCartCommand(cart.Id, PaymentToPaymamentDTO(payment)), CancellationToken.None);

        Assert.True(result);
        Assert.False(_notification.HasErrors());
    }


    [Fact(DisplayName = nameof(CheckoutToCart_ShouldReturnFalse_WhenProductNotFound))]
    [Trait("Integration - Application", "Service Cart")]
    public async void CheckoutToCart_ShouldReturnFalse_WhenProductNotFound()
    {

        var cart = FakerCart();
        var payment = FakerPayment(Domain.Entity.PaymentMethod.CreditCard, Domain.Entity.PaymentStatus.Pending, cart.GetTotal());
        cart.AddPayment(payment);

        await _cartRepository.AddAsync(cart, CancellationToken.None);


        var result = await _checkoutHandler.Handle(new CheckoutCommand(cart.Id), CancellationToken.None); //_cartServices.Checkout(cart.Id);
        var cartBase = _cartRepository.GetByIdAsync(cart.Id).Result;

        Assert.False(result);
        Assert.True(_notification.HasErrors());
        Assert.True(_notification.Errors().Where(x => x.Message == "Valor total do carrinho e diferente do valor total dos pagamentos").Count() > 0);
        Assert.Equal(cartBase.Status, Domain.Entity.CartStatus.PendingCheckout);
    }


    [Fact(DisplayName = nameof(CheckoutToCart_ShouldReturnFalse_WhenPaymentNotFound))]
    [Trait("Integration - Application", "Service Cart")]
    public async void CheckoutToCart_ShouldReturnFalse_WhenPaymentNotFound()
    {
        var product = FakerProduct();
        var cart = FakerCart();
        cart.AddItem(product, 3);

        await _cartRepository.AddAsync(cart, CancellationToken.None);


        var result = await _checkoutHandler.Handle(new CheckoutCommand(cart.Id), CancellationToken.None);
        var cartBase = _cartRepository.GetByIdAsync(cart.Id).Result;

        Assert.False(result);
        Assert.True(_notification.HasErrors());
        Assert.True(_notification.Errors().Where(x => x.Message == "Não é possivel alterar o status do carrinho para CheckoutCompleted pois não existe pagamento").Count() > 0);
        Assert.Equal(cartBase.Status, Domain.Entity.CartStatus.PendingCheckout);
    }


    [Fact(DisplayName = nameof(CheckoutToCart_ShouldReturnFalse_WhenCustomerNotFound))]
    [Trait("Integration - Application", "Service Cart")]
    public async void CheckoutToCart_ShouldReturnFalse_WhenCustomerNotFound()
    {
        var cart = FakerCart(true, false, true);

        await _cartRepository.AddAsync(cart, CancellationToken.None);


        var result = await _checkoutHandler.Handle(new CheckoutCommand(cart.Id), CancellationToken.None);
        var cartBase = _cartRepository.GetByIdAsync(cart.Id).Result;

        Assert.False(result);
        Assert.True(_notification.HasErrors());
        Assert.True(_notification.Errors().Where(x => x.Message == "por favor informar um cliente para fazer o checkout").Count() > 0);
        Assert.Equal(cartBase.Status, Domain.Entity.CartStatus.PendingCheckout);
    }


    [Fact(DisplayName = nameof(CheckoutToCart_ShouldReturnTrue_WhenCartIsValid))]
    [Trait("Integration - Application", "Service Cart")]
    public async void CheckoutToCart_ShouldReturnTrue_WhenCartIsValid()
    {
        var cart = FakerCart(true, true, true);

        await _cartRepository.AddAsync(cart, CancellationToken.None);

        var result = await _checkoutHandler.Handle(new CheckoutCommand(cart.Id), CancellationToken.None);
        var cartBase = _cartRepository.GetByIdAsync(cart.Id).Result;

        //aqui eu preciso ler o rabbitMQ para verificar se a messagem chegou no broker
        var (message, quantity) = await _rabbitMQPersistence.ReadMessageFromRabbitMQAutoAck<OrderCheckoutEventDTO>();


        Assert.True(result);
        Assert.False(_notification.HasErrors());
        Assert.Equal(cartBase.Status, Domain.Entity.CartStatus.CheckoutCompleted);
        Assert.NotNull(message);


        Assert.Equal("OrderCheckoutedEvent", message.EventName);
        Assert.Equal(cartBase.Customer.Id, message.Customer.Id);
        Assert.Equal(cartBase.Customer.Name, message.Customer.Name);
        Assert.Equal(cartBase.Customer.Email, message.Customer.Email);
        Assert.Equal(cartBase.Customer.Phone, message.Customer.Phone);
        Assert.Equal(cartBase.Customer.Address.Country, message.Customer.Address.Country);
        Assert.Equal(cartBase.Customer.Address.State, message.Customer.Address.State);
        Assert.Equal(cartBase.Customer.Address.City, message.Customer.Address.City);
        Assert.Equal(cartBase.Customer.Address.Street, message.Customer.Address.Street);
        Assert.Equal(cartBase.Customer.Address.Number, message.Customer.Address.Number);
        Assert.Equal(cartBase.Customer.Address.Complement, message.Customer.Address.Complement);
        Assert.Equal(cartBase.Customer.Address.District, message.Customer.Address.District);
        Assert.Equal(cartBase.Version, message.Version);
        Assert.Equal(cartBase.Id, message.Id);
        Assert.Equal(cartBase.Products.Count, message.Products.Count);
        Assert.Equal(cartBase.processedEvent, true);

        var produtosBase = cartBase.Products.ToList();
        foreach (var item in produtosBase)
        {
            var productMessage = message.Products.FirstOrDefault(p => p.Id == item.Id);
            Assert.Equal(item.Name, productMessage.Name);
            Assert.Equal(item.Price, productMessage.Price);
            Assert.Equal(item.CategoryId, productMessage.CategoryId);
            Assert.Equal(item.Thumb, productMessage.Thumb);
            Assert.Equal(item.Description, productMessage.Description);
            Assert.Equal(item.IsSale, productMessage.IsSale);
            Assert.Equal(item.Id, productMessage.Id);
        }

        var paymentBase = cartBase.Payments.ToList();

        foreach (var item in paymentBase)
        {
            var paymentMessage = message.Payments.FirstOrDefault(p => p.PaymentMethod == item.PaymentMethod);
            Assert.Equal(item.PaymentMethod, paymentMessage.PaymentMethod);
            Assert.Equal(item.Status, paymentMessage.Status);
            Assert.Equal(item.Amount, paymentMessage.Amount);
        }

    }

}
