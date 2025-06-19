using MediatR;
using Mshop.Core.Base;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Entity;
using Mshop.Infra.Data.Interface;
using MessageCore = Mshop.Core.Message;

namespace Mshop.Application.Services.Cart.Commands.Handlers
{
    public class AddAddressToCartHandler : BaseCommand, IRequestHandler<AddAddressToCartCommand, bool>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IDomainEventPublisher _publishService;

        public AddAddressToCartHandler(
            ICartRepository cartRepository, 
            IDomainEventPublisher publishService, 
            MessageCore.INotification notification) : base(notification)
        {
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
        }
        public async Task<bool> Handle(AddAddressToCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CartId);
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
            customer.AddAdress(new Address(request.Address.Street, request.Address.Number, request.Address.Complement, request.Address.Neighborhood,
            request.Address.City, request.Address.State, request.Address.PostalCode, request.Address.Country));

            customer.IsValid(Notifications);
            if (TheareErrors()) return false;
            cart.UpdateCustomer(customer);
            cart.IsValid(Notifications);
            if (TheareErrors()) return false;

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            await _publishService.PublishAsync(cart.Events);
            return true;
        }
    }
}
