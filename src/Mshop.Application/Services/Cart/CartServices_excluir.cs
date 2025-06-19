using Mshop.Application.Commons.DTO;
using Mshop.Application.Commons.Response;
using Mshop.Application.Interface;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Entity;
using Mshop.Domain.Event;
using Mshop.Infra.Data.Interface;
using Message = Mshop.Core.Message;

namespace Mshop.Application.Services.Cart
{
    public class CartServices_excluir: BaseService, ICartServices
    {
        private readonly ICartRepository _cartRepository;
        private readonly Message.INotification _notification;
        private readonly IProductConsumer _productConsumer;
        private readonly ICustomerConsumer _customerGPRc;
        private readonly IDomainEventPublisher _publishService;
        public CartServices_excluir(
            ICartRepository cartRepository, 
            IProductConsumer productGrpc, 
            ICustomerConsumer customerGrpc,
            IDomainEventPublisher publishService,
            Message.INotification notification) : base(notification)
        {
            _cartRepository = cartRepository;
            _productConsumer = productGrpc;
            _customerGPRc = customerGrpc;
            _notification = notification;
            _publishService = publishService;
        }

        public async Task<Result<CartResponse>> AddItemToCartAsync(Guid cartId, Guid productId, int quantity, CancellationToken cancellationToken = default)
        {
            
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if(cart is null || cartId == Guid.Empty) 
                cart = await NewCart(cartId);

            var productGRPc = await _productConsumer.GetProductByIdAsync(productId);
            if(productGRPc is null)
            {
                Notificar("Não foi possivel encontrar o produto");
                return Result<CartResponse>.Error(Notifications);
            }

            var product = new Product(
                productId,
                productGRPc.Description,
                productGRPc.Name,
                productGRPc.Price,
                productGRPc.IsPromotion,
                productGRPc.CategoryId,
                productGRPc.Category,
                productGRPc.Thumb,
                quantity); 

            product.IsValid(_notification);
            if (TheareErrors()) return Result<CartResponse>.Error(Notifications);
            
            cart.AddItem(product, quantity);     
            cart.IsValid(_notification);
            if (TheareErrors()) return Result<CartResponse>.Error(Notifications);

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);

            await _publishService.PublishAsync(cart.Events);
          
            return Result<CartResponse>.Success(new CartResponse(
                cart.Id,
                CartResponse.ProductTOProductDTO(cart.Products),
                CartResponse.CustomerTOCustomerDTO(cart.Customer)));
        }

        public async Task<bool> RemoveItemFromCart(Guid cartId, Guid productId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }

            cart.RemoveItem(productId);
            cart.IsValid(_notification);
            if (TheareErrors()) return false;

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            await _publishService.PublishAsync(cart.Events);
            return true;
        }

        public async Task<bool> RemoveQuantityFromCart(Guid cartId, Guid productId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }
            cart.RemoveQuantity(productId);
            cart.IsValid(_notification);
            if (TheareErrors()) return false;
            
            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            await _publishService.PublishAsync(cart.Events);
            return true;
        }

        public async Task<bool> ClearCart(Guid cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }

            cart.ClearCart();
            cart.IsValid(_notification);

            if (TheareErrors()) return false;

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            await _publishService.PublishAsync(cart.Events);
            return true;
        }

        public async Task<bool> AddCustomer(Guid cartId, Guid customerId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }

            if(cart.Customer != null && cart.Customer.Id != Guid.Empty && cart.Customer.Id != customerId)
            {
                Notificar("Desculpa houve um error");
                return false;
            }

            var customerGRPc = await _customerGPRc.GetCustomerByIdAsync(customerId);
            if (customerGRPc is null)
            {
                Notificar("Não foi possivel encontrar o cliente");
                return false;
            }

            //implementar o GRPC customer
            var customer = new Customer(customerId,customerGRPc.Name,customerGRPc.Email,customerGRPc.Phone);
            customer.IsValid(_notification);
            if (TheareErrors()) return false;

            cart.UpdateCustomer(customer);
            cart.IsValid(_notification);
            if (TheareErrors()) return false;

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            await _publishService.PublishAsync(cart.Events);
            return true;
        }

        public async Task<bool> AddAddressToCart(Guid cartId, AddressDTO address)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }

            var customer = cart.Customer;
            if (customer is null)
            {
                Notificar("Por favor faça o Checkout");
                return false;
            }
                

            customer.AddAdress(new Address(address.Street, address.Number, address.Complement, address.Neighborhood,
                                             address.City, address.State, address.PostalCode, address.Country));
            customer.IsValid(_notification);
            if (TheareErrors()) return false;

            cart.UpdateCustomer(customer);
            cart.IsValid(_notification);
            if (TheareErrors()) return false;

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            await _publishService.PublishAsync(cart.Events);
            return true;
        }

        public async Task<CartResponse?> GetCartDetails(Guid cartId, CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);

            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return null;
            }

            return new CartResponse(
                cart.Id,
                CartResponse.ProductTOProductDTO(cart.Products),
                CartResponse.CustomerTOCustomerDTO(cart.Customer));
        }

        public async Task<CartResponse?> GetCartByCustomerId(Guid customerId, CancellationToken cancellationToken = default)
        {
            var cart = await _cartRepository.GetByCustomerId(customerId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return null;
            }

            return new CartResponse(
               cart.Id,
               CartResponse.ProductTOProductDTO(cart.Products),
               CartResponse.CustomerTOCustomerDTO(cart.Customer));
        }

        public async Task<bool> AddPayment(Guid cartId, Payment payment)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }

            cart.AddPayment(payment);
            cart.IsValid(_notification);
            if (TheareErrors()) return false;

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            await _publishService.PublishAsync(cart.Events);
            return true;
        }

        public async Task<bool> Checkout(Guid cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }
            cart.Checkout();
            cart.IsValid(_notification);
            if (TheareErrors()) return false;
            await _cartRepository.UpdateAsync(cart, CancellationToken.None);

            //aqui mandar para o message broker 
            var resultBroker = await _publishService.PublishAsync(cart.Events);

            if (resultBroker)
            {
                cart.ConfirmEvent();
                await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            }

            return true;
        }

        private async Task<Domain.Entity.Cart> NewCart(Guid cartId)
        {
            var cart = new Domain.Entity.Cart(cartId);
            await _cartRepository.AddAsync(cart, CancellationToken.None);
            cart.RegisterEvent(new OrderCreatedEvent(cart));
            return cart;
        }

    }
}
