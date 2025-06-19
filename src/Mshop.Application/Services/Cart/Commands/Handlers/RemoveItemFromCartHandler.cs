using MediatR;
using Mshop.Core.Base;
using Mshop.Core.Message.DomainEvent;
using Mshop.Infra.Data.Interface;
using MessageCore = Mshop.Core.Message;

namespace Mshop.Application.Services.Cart.Commands.Handlers
{
    public class RemoveItemFromCartHandler : BaseCommand, IRequestHandler<RemoveItemFromCartCommand, bool>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IDomainEventPublisher _publishService;
        public RemoveItemFromCartHandler(
            ICartRepository cartRepository,
            IDomainEventPublisher publishService,
            MessageCore.INotification notification)
            : base(notification)
        {
            _cartRepository = cartRepository;
            _publishService = publishService;
        }
        public async Task<bool> Handle(RemoveItemFromCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }

            cart.RemoveItem(request.ProductId);
            cart.IsValid(Notifications);
            if (TheareErrors()) return false;

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            await _publishService.PublishAsync(cart.Events);
            return true;
        }
    }
}
