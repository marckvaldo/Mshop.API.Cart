using MediatR;
using Mshop.Application.Commons.Response;
using Mshop.Application.Interface;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Entity;
using Mshop.Domain.Event;
using Mshop.Infra.Data.Interface;
using MessageCore = Mshop.Core.Message;

namespace Mshop.Application.Services.Cart.Commands.Handlers
{
    public class AddItemToCartHandler : BaseCommand, IRequestHandler<AddItemToCartCommand, Result<CartResponse>>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductConsumer _productConsumer;
        private readonly IDomainEventPublisher _publishService;

        public AddItemToCartHandler(
            ICartRepository cartRepository, 
            IProductConsumer productConsumer,
            IDomainEventPublisher publishService,
            MessageCore.INotification notification) : base(notification)
        {
            _cartRepository = cartRepository;
            _productConsumer = productConsumer;
            _publishService = publishService;
        }
        public async Task<Result<CartResponse>> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
        {

            var cart = await _cartRepository.GetByIdAsync(request.CartId);
            if (cart is null || request.CartId == Guid.Empty)
                cart = await NewCart(request.CartId);

            var productGRPc = await _productConsumer.GetProductByIdAsync(request.ProductId);
            if (productGRPc is null)
            {
                Notificar("Não foi possivel encontrar o produto");
                return Result<CartResponse>.Error(Notifications);
            }

            var product = new Product(
                request.ProductId,
                productGRPc.Description,
                productGRPc.Name,
                productGRPc.Price,
                productGRPc.IsPromotion,
                productGRPc.CategoryId,
                productGRPc.Category,
                productGRPc.Thumb,
                request.Quantity);

            product.IsValid(Notifications);
            if (TheareErrors()) return Result<CartResponse>.Error(Notifications);

            cart.AddItem(product, request.Quantity);
            cart.IsValid(Notifications);
            if (TheareErrors()) return Result<CartResponse>.Error(Notifications);

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);

            await _publishService.PublishAsync(cart.Events);

            return Result<CartResponse>.Success(new CartResponse(
                cart.Id,
                CartResponse.ProductTOProductDTO(cart.Products),
                CartResponse.CustomerTOCustomerDTO(cart.Customer)));

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
