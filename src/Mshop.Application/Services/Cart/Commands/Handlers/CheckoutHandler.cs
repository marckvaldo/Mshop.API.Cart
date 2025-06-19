using MediatR;
using Mshop.Core.Base;
using Mshop.Core.Message.DomainEvent;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.Services.Cart.Commands.Handlers
{
    public class CheckoutHandler : BaseCommand, IRequestHandler<CheckoutCommand, bool>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IDomainEventPublisher _publishService;
        public CheckoutHandler(
            ICartRepository cartRepository,
            IDomainEventPublisher publishService,
            Mshop.Core.Message.INotification notification) : base(notification)
        {
            _cartRepository = cartRepository;
            _publishService = publishService;
        }
        public async Task<bool> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }
            cart.Checkout();
            cart.IsValid(Notifications);
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
    }
}
