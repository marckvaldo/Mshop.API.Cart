using MediatR;
using Mshop.Core.Base;
using MessageCore = Mshop.Core.Message;
using Mshop.Core.Message.DomainEvent;
using Mshop.Domain.Entity;
using Mshop.Infra.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mshop.Application.Commons.DTO;

namespace Mshop.Application.Services.Cart.Commands.Handlers
{
    public class AddPaymentHandler : BaseCommand, IRequestHandler<AddPaymentToCartCommand, bool>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IDomainEventPublisher _publishService;

        public AddPaymentHandler(
            ICartRepository cartRepository, 
            IDomainEventPublisher publishService, 
            MessageCore.INotification notification): base(notification)
        {
            _cartRepository = cartRepository;
            _publishService = publishService;
        }

        public async Task<bool> Handle(AddPaymentToCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetByIdAsync(request.CartId);
            if (cart is null)
            {
                Notificar("Não foi possivel encontrar o carrinho de compras");
                return false;
            }

            if (request.Payment.PaymentMethod == PaymentMethod.BoletoBancario)
            {
                var payment = new Payment(request.Payment.Amount,
                    request.Payment.PaymentMethod,
                    request.Payment.BoletoNumber,
                    request.Payment.BoletoDueDate);

                payment.UpdateStatus(request.Payment.PaymentStatus);
                cart.AddPayment(payment);
            }
            else
            {
                var payment = new Payment(request.Payment.Amount,
                    request.Payment.PaymentMethod,
                    request.Payment.Installments,
                    request.Payment.CardToken);

                payment.UpdateStatus(request.Payment.PaymentStatus);
                cart.AddPayment(payment);
            }

           
            cart.IsValid(Notifications);
            if (TheareErrors()) return false;

            await _cartRepository.UpdateAsync(cart, CancellationToken.None);
            await _publishService.PublishAsync(cart.Events);
            return true;
        }
    }
}
