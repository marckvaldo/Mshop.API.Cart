using Mshop.Application.Commons.DTO;
using Mshop.Application.Commons.Response;
using Mshop.Application.Interface;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Message = Mshop.Core.Message;
using Mshop.Domain.Entity;
using Mshop.Infra.Data.Interface;
using Mshop.Core.Message;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Event;

namespace Mshop.Application.Services.Cart
{
    public class CartServices: BaseService, ICartServices
    {
        private readonly ICartRepository _cartRepository;
        private readonly Message.INotification _notification;
        private readonly IProductConsumer _productConsumer;
        private readonly ICustomerConsumer _customerGPRc;
        private readonly IDomainEventPublisher _publishService;
        public CartServices(
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
        }

        public async Task<Result<CartDTO>> AddItemToCart(Guid cartId, Guid productId, int quantity)
        {
            
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if(cart is null && cartId == Guid.Empty) 
                cart = await NewCart();
            else 
                cart = await NewCart(cartId);

            var productGRPc = await _productConsumer.GetProductByIdAsync(productId);
            if(productGRPc is null)
            {
                Notificar("Não foi possivel encontrar o produto");
                return Result<CartDTO>.Error(Notifications);
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
            if (TheareErrors()) return Result<CartDTO>.Error(Notifications);
            
            cart.AddItem(product, quantity);     
            cart.IsValid(_notification);
            if (TheareErrors()) return Result<CartDTO>.Error(Notifications);

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
          
            return Result<CartDTO>.Success(new CartDTO(
                cart.Id,
                CartDTO.ProductTOProductDTO(cart.Products),
                CartDTO.CustomerTOCustomerDTO(cart.Customer)));
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
                Notificar("Por favor faça o Chekout");
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

            return true;
        }

        public async Task<CartDTO?> GetCartDetails(Guid cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);

            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return null;
            }

            return new CartDTO(
                cart.Id,
                CartDTO.ProductTOProductDTO(cart.Products),
                CartDTO.CustomerTOCustomerDTO(cart.Customer));
        }

        public async Task<CartDTO?> GetCartByCustomer(Guid customerId)
        {
            var cart = await _cartRepository.GetByCustomerId(customerId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return null;
            }

            return new CartDTO(
               cart.Id,
               CartDTO.ProductTOProductDTO(cart.Products),
               CartDTO.CustomerTOCustomerDTO(cart.Customer));
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
            cart.UpdateStatus(CartStatus.CheckoutCompleted);
            cart.IsValid(_notification);
            if (TheareErrors()) return false;
            await _cartRepository.UpdateAsync(cart, CancellationToken.None);

            //aqui mandar para o message broker 
            var result = await _publishService.PublishAsync(new OrderCreateEvent(cart));
            if (!result)
                return false;

            return true;
        }

        private async Task<Domain.Entity.Cart> NewCart()
        {
            var cart = new Domain.Entity.Cart(Guid.Empty);
            await _cartRepository.AddAsync(cart, CancellationToken.None);
            return cart;
        }

        private async Task<Domain.Entity.Cart> NewCart(Guid cartId)
        {
            var cart = new Domain.Entity.Cart(cartId);
            await _cartRepository.AddAsync(cart, CancellationToken.None);
            return cart;
        }

    }
}
