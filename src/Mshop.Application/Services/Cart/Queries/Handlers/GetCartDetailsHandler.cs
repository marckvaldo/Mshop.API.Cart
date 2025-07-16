using MediatR;
using Mshop.Application.Commons.Response;
using Mshop.Application.Interface;
using Mshop.Core.Base;
using Mshop.Core.DomainObject;
using Mshop.Domain.Entity;
using Mshop.Infra.Data.Interface;
using Messagecore = Mshop.Core.Message;

namespace Mshop.Application.Services.Cart.Queries.Handlers
{
    public class GetCartDetailsHandler : BaseQuery, IRequestHandler<GetCartDetailsQuery, Result<CartResponse>>
    {
        private readonly Messagecore.INotification _notification;
        private readonly ICartRepository _cartRepository;
        public GetCartDetailsHandler(Messagecore.INotification notification,ICartRepository cartRepository) : base(notification)
        {
            _cartRepository = cartRepository;
        }

        public async Task<Result<CartResponse>> Handle(GetCartDetailsQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CartId);

            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return Result<CartResponse>.Error(Notifications);
            }

            return Result<CartResponse>.Success(new CartResponse(
                cart.Id,
                CartResponse.ProductTOProductDTO(cart.Products),
                CartResponse.CustomerTOCustomerDTO(cart.Customer),
                CartResponse.PaymentToPaymentDTO(cart.Payments)));

        }
    }
}
