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
    public class GetCartByCustomerIdHandler : BaseQuery, IRequestHandler<GetCartByCustomerIdQuery, Result<CartResponse>> 
    {
        private readonly ICartRepository _cartRepository;
        public GetCartByCustomerIdHandler(Messagecore.INotification notification, ICartRepository cartRepository) : base(notification)
        {
            _cartRepository = cartRepository;
        }
        public async Task<Result<CartResponse>> Handle(GetCartByCustomerIdQuery request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByCustomerId(request.CustomerId);
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
