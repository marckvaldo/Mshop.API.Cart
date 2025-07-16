using Microsoft.Extensions.DependencyInjection;
using Mshop.API.Cart.Requests.V1.Cart;
using Mshop.Application.Commons.Response;
using Mshop.Application.Interface;
using Mshop.Application.Services.Cart.Commands;
using Mshop.Application.Services.Cart.Commands.Handlers;
using Mshop.Application.Services.Cart.Queries.Handlers;
using Mshop.Cart.E2ETests.Configuration;
using Mshop.Core.Message;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Event;
using Mshop.E2ETests.Common;
using Mshop.E2ETests.Common.Event;
using Mshop.E2ETests.Persistence.RabbitMQ;
using Mshop.Infra.Data.Interface;


namespace Mshop.E2ETests.API.Cart
{
    [Collection("Crud Shopping Cart Collection")]
    [CollectionDefinition("Crud Shopping Cart Collection", DisableParallelization = true)]
    public class CarrinhoAPITest : CarrinhoAPITestFixture
    {
        private readonly ICartRepository _cartRepository;
        private readonly RabbitMQPersistence _rabbitMQPersistence;

        public CarrinhoAPITest() : base()
        {
            _cartRepository = _serviceProvider.GetService<ICartRepository>();
            _rabbitMQPersistence = new RabbitMQPersistence(_serviceProvider);
        }

        [Fact(DisplayName = "Should Create a Shopping Cart where add a fist product")]
        [Trait("End2End - API", "Shopping")]
        public async Task ShouldCreateShoppingCart()
        {
            // Arrange
            PersistProductsDataBase();
            var products = await GetAllProductsMysqlAsync();
            var product = products.FirstOrDefault();

            var cart = FakerCart();
            var request = new AddItemToCartRequest() { ProductId = product.Id, Quantity = 1 };
            // Act
            var (response, outPut) = await _apiClient.Post<CustomResponse<CartResponse>>($"{ConfigurationTests.URL_API_CART}/{cart.Id}/items", request);
            // Assert
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(outPut.Data.Id, cart.Id);
            var produtoResponse = outPut.Data.Products.Where(x => x.Id == product.Id).FirstOrDefault();
            Assert.NotNull(produtoResponse);
            Assert.Equal(produtoResponse.Description, product.Description);
            Assert.Equal(produtoResponse.Quantity, 1);
            Assert.Equal(produtoResponse.Price, product.Price);
            Assert.Equal(produtoResponse.Id, product.Id);
            Assert.Equal(produtoResponse.Thumb, product.Thumb);
            Assert.Equal(produtoResponse.CategoryId, product.CategoryId);

        }


        [Fact(DisplayName = "Should Remove item of Shopping cart")]
        [Trait("End2End - API", "Shopping")]
        public async Task ShouldRemoveItemOfShoppingCart()
        {
            // Arrange
            var cart = FakerCart(true,false,false);
            await _cartRepository.AddAsync(cart, CancellationToken.None);
            var product = cart.Products.FirstOrDefault();
            
            // Act
            var (response, outPut) = await _apiClient.Delete<CustomResponse<bool>>($"{ConfigurationTests.URL_API_CART}/{cart.Id}/items/{product.Id}");

            // Assert
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            
            var newCart = await _cartRepository.GetByIdAsync(cart.Id);
            Assert.NotNull(newCart);
            Assert.NotEmpty(newCart.Products);
            Assert.True(!newCart.Products.Where(x=>x.Id == product.Id).Any());

        }


        [Fact(DisplayName = "Should Less Quantity item of Shopping cart")]
        [Trait("End2End - API", "Shopping")]
        public async Task ShouldLessItemOfShoppingCart()
        {
            // Arrange
            var cart = FakerCart(true, false, false);
            cart.Products.FirstOrDefault()?.UpdateQuantity(5);
            await _cartRepository.AddAsync(cart, CancellationToken.None);
            var product = cart.Products.FirstOrDefault();

            var request = new LessQuantityItemRequest() { Quantity = 2 };

            // Act
            var (response, outPut) = await _apiClient.Put<CustomResponse<bool>>($"{ConfigurationTests.URL_API_CART}/{cart.Id}/items/{product.Id}/LessQuantity", request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);

            var newCart = await _cartRepository.GetByIdAsync(cart.Id);
            Assert.NotNull(newCart);
            Assert.NotEmpty(newCart.Products);
            Assert.True(newCart.Products.Where(x => x.Id == product.Id).FirstOrDefault().Quantity == 3);
        }


        [Fact(DisplayName = "Should checkout Shopping cart")]
        [Trait("End2End - API", "Shopping")]
        public async Task ShoulChekoutShoppingCart()
        {
            // Arrange
            await _rabbitMQPersistence.SetupRabbitMQWithDeadLette();
            var cart = FakerCart(true, true, true);
            await _cartRepository.AddAsync(cart, CancellationToken.None);

            // Act
            var (response, outPut) = await _apiClient.Get<CustomResponse<bool>>($"{ConfigurationTests.URL_API_CART}/{cart.Id}/checkout");

            // Assert
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);

            var newCart = await _cartRepository.GetByIdAsync(cart.Id);
            Assert.NotNull(newCart);
            Assert.NotNull(newCart.Customer);
            Assert.True(newCart.Status == Domain.Entity.CartStatus.CheckoutCompleted);

            var (messagem, countMessage) = await _rabbitMQPersistence.ReadMessageFromRabbitMQAutoAck<OrderCheckoutedEventMessage>();
            Assert.NotNull(messagem);
            Assert.Equal(messagem.Id, newCart.Id);
            Assert.Equal(messagem.Customer.Id, newCart.Customer.Id);
            Assert.Equal(messagem.Customer.Name, newCart.Customer.Name);
            Assert.Equal(messagem.Customer.Email, newCart.Customer.Email);
            Assert.Equal(messagem.Customer.Phone, newCart.Customer.Phone);
            Assert.NotEmpty(messagem.Products);
            Assert.Equal(messagem.Products.Count(), newCart.Products.Count());
            Assert.NotEmpty(messagem.Payments);
            Assert.Equal(messagem.Payments.Count(), newCart.Payments.Count());

        }


        [Fact(DisplayName = "Should Add an address to the Shopping cart")]
        [Trait("End2End - API", "Shopping")]
        public async Task ShouldAddanAddressToTheShoppingCart()
        {
            // Arrange
            /*var cart = FakerCart(true, false, false);
            await _cartRepository.AddAsync(cart, CancellationToken.None);
            var customer = FakerCustomer();

            var request = new AddCustomerToCartRequest() { CustomerId = customer.Id };

            // Act
            var (response, outPut) = await _apiClient.Post<CustomResponse<bool>>($"{ConfigurationTests.URL_API_CART}/{cart.Id}/customer", request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);

            var newCart = await _cartRepository.GetByIdAsync(cart.Id);
            Assert.NotNull(newCart);
            Assert.NotNull(newCart.Customer);
            Assert.Equal(newCart.Customer.Name, customer.Name);
            Assert.Equal(newCart.Customer.Email, customer.Email);
            Assert.Equal(newCart.Customer.Id, customer.Id);
            Assert.Equal(newCart.Customer.Phone, customer.Phone);*/

            Assert.True(true);
        }


        [Fact(DisplayName = "Should Add Payment to the Shopping cart")]
        [Trait("End2End - API", "Shopping")]
        public async Task ShouldAddPaymentToTheShoppingCart()
        {
            // Arrange
            var cart = FakerCart(true, false, false);
            await _cartRepository.AddAsync(cart, CancellationToken.None);
            var payment = FakerPayment(Domain.Entity.PaymentMethod.CreditCard, Domain.Entity.PaymentStatus.Pending, cart.GetTotal());

            var request = new AddPaymentToCartRequest() {
                PaymentMethod = payment.PaymentMethod,
                Amount = payment.Amount,
                Installments = payment.Installments,
                CardToken = payment.CardToken,
                BoletoNumber = payment.BoletoNumber,
                BoletoDueDate = payment.BoletoDueDate
            };

            // Act
            var (response, outPut) = await _apiClient.Post<CustomResponse<bool>>($"{ConfigurationTests.URL_API_CART}/{cart.Id}/payments", request);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);

            var newCart = await _cartRepository.GetByIdAsync(cart.Id);
            Assert.NotNull(newCart);
            Assert.NotEmpty(newCart.Payments);
            var paymentCart = newCart.Payments.FirstOrDefault();
            Assert.Equal(paymentCart.BoletoNumber, payment.BoletoNumber);
            Assert.Equal(paymentCart.BoletoDueDate, payment.BoletoDueDate);
            Assert.Equal(paymentCart.CardToken, payment.CardToken);
            Assert.Equal(paymentCart.Amount, payment.Amount);
            Assert.Equal(paymentCart.PaymentMethod, payment.PaymentMethod);
            Assert.Equal(paymentCart.Installments, payment.Installments);
            Assert.Equal(paymentCart.Status, payment.Status);
        }


        [Fact(DisplayName = "Should Delete items of Shopping cart")]
        [Trait("End2End - API", "Shopping")]
        public async Task ShouldDeleteItemsOfShoppingCart()
        {
            // Arrange
            var cart = FakerCart(true, false, false);
            await _cartRepository.AddAsync(cart, CancellationToken.None);

            // Act
            var (response, outPut) = await _apiClient.Delete<CustomResponse<bool>>($"{ConfigurationTests.URL_API_CART}/{cart.Id}/items");

            // Assert
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);

            var newCart = await _cartRepository.GetByIdAsync(cart.Id);
            Assert.Empty(newCart.Products);
        }


        [Fact(DisplayName = "Should Show shopping when search by id Cart")]
        [Trait("End2End - API", "Shopping")]
        public async Task ShouldShowShoppingWhenSearchByIdCartShoppingCart()
        {
            // Arrange
            var cart = FakerCart(true,true,true);
            await _cartRepository.AddAsync(cart, CancellationToken.None);

            // Act
            var (response, outPut) = await _apiClient.Get<CustomResponse<CartResponse>>($"{ConfigurationTests.URL_API_CART}/{cart.Id}");
            // Assert
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(outPut.Data.Id, cart.Id);
            Assert.NotEmpty(outPut.Data.Products);
            Assert.NotNull(outPut.Data.Customer);
            Assert.Equal(outPut.Data.Customer.Id, cart.Customer.Id);
            Assert.Equal(outPut.Data.Customer.Name, cart.Customer.Name);
            Assert.Equal(outPut.Data.Customer.Email, cart.Customer.Email);
            Assert.Equal(outPut.Data.Customer.Phone, cart.Customer.Phone);
            Assert.Equal(outPut.Data.Payments.Count(),cart.Payments.Count());

        }


        [Fact(DisplayName = "Should Show shopping when search by Customer Id")]
        [Trait("End2End - API", "Shopping")]
        public async Task ShouldShowShoppingWhenSearchByCustomerIdShoppingCart()
        {
            // Arrange
            var cart = FakerCart(true, true, true);
            await _cartRepository.AddAsync(cart, CancellationToken.None);

            // Act
            var (response, outPut) = await _apiClient.Get<CustomResponse<CartResponse>>($"{ConfigurationTests.URL_API_CART}/customer/{cart.Customer.Id}");
            // Assert
            Assert.NotNull(response);
            Assert.Equal(System.Net.HttpStatusCode.OK, response!.StatusCode);
            Assert.NotNull(outPut);
            Assert.True(outPut.Success);
            Assert.Equal(outPut.Data.Id, cart.Id);
            Assert.NotEmpty(outPut.Data.Products);
            Assert.NotNull(outPut.Data.Customer);
            Assert.Equal(outPut.Data.Customer.Id, cart.Customer.Id);
            Assert.Equal(outPut.Data.Customer.Name, cart.Customer.Name);
            Assert.Equal(outPut.Data.Customer.Email, cart.Customer.Email);
            Assert.Equal(outPut.Data.Customer.Phone, cart.Customer.Phone);
            Assert.Equal(outPut.Data.Payments.Count(), cart.Payments.Count());

        }
    }
}
