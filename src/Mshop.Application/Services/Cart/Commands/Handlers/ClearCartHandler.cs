using MediatR;
using Mshop.Core.Base;
using Mshop.Core.Message.DomainEvent;
using Mshop.Infra.Data.Interface;
using MessageCore = Mshop.Core.Message;

namespace Mshop.Application.Services.Cart.Commands.Handlers
{
    public class ClearCartHandler : BaseCommand, IRequestHandler<ClearCartCommand, bool>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IDomainEventPublisher _publishService;

        public ClearCartHandler(ICartRepository cartRepository, IDomainEventPublisher publishService, MessageCore.INotification notification)
            : base(notification)
        {
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
        }

        public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CarId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }

            cart.ClearCart();
            cart.IsValid(Notifications);

            if (TheareErrors()) return false;

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            await _publishService.PublishAsync(cart.Events);
            return true;
        }
    }
}
