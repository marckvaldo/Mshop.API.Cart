using MediatR;
using Mshop.Application.Interface;
using Mshop.Core.Base;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Entity;
using Mshop.Infra.Data.Interface;

namespace Mshop.Application.Services.Cart.Commands.Handlers
{
    public class AddCustomerHandler : BaseCommand, IRequestHandler<AddCustomerToCartCommand, bool>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICustomerConsumer _customerGPRc;
        private readonly IDomainEventPublisher _publishService;

        public AddCustomerHandler(Core.Message.INotification notification, ICartRepository cartRepository, ICustomerConsumer customerGPRc, IDomainEventPublisher publishService) : base(notification)
        {
            _cartRepository = cartRepository ?? throw new ArgumentNullException(nameof(cartRepository));
            _customerGPRc = customerGPRc ?? throw new ArgumentNullException(nameof(_customerGPRc));
            _publishService = publishService ?? throw new ArgumentNullException(nameof(publishService));
        }

        public async Task<bool> Handle(AddCustomerToCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }

            if (cart.Customer != null && cart.Customer.Id != Guid.Empty && cart.Customer.Id != request.CustomerId)
            {
                Notificar("Desculpa houve um error");
                return false;
            }

            var customerGRPc = await _customerGPRc.GetCustomerByIdAsync(request.CustomerId);
            if (customerGRPc is null)
            {
                Notificar("Não foi possivel encontrar o cliente");
                return false;
            }

            //implementar o GRPC customer
            var customer = new Customer(request.CustomerId, customerGRPc.Name, customerGRPc.Email, customerGRPc.Phone);
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
