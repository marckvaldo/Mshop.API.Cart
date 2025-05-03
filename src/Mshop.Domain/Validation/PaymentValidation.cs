using FluentValidation;
using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Validation
{
    public class PaymentValidation : AbstractValidator<Payment>
    {
        public PaymentValidation()
        {
            When(p => p.PaymentMethod == PaymentMethod.CreditCard, () =>
            {
                RuleFor(p => p.CardToken)
                    .NotEmpty().WithMessage("Card number is required for credit card payments.")
                    .CreditCard().WithMessage("Invalid card number.");

                RuleFor(p => p.Installments)
                    .InclusiveBetween(1, 12).WithMessage("Installments must be between 1 and 12.");
            });

            // Validação para boleto bancário
            When(p => p.PaymentMethod == PaymentMethod.BoletoBancario, () =>
            {
                RuleFor(p => p.BoletoNumber)
                    .NotEmpty().WithMessage("Bank slip number is required for bank slip payments.");
                    //.Length(20).WithMessage("Bank slip number must be exactly 20 characters.");

                RuleFor(p => p.BoletoDueDate)
                    .NotEmpty().WithMessage("Bank slip due date is required for bank slip payments.")
                    .GreaterThan(DateTime.UtcNow).WithMessage("Bank slip due date must be in the future.");
            });


            RuleFor(p => p.PaymentMethod)
                .IsInEnum().WithMessage("Invalid payment method.");

            RuleFor(payment => payment.Amount)
                .GreaterThan(0).WithMessage("O valor do pagamento deve ser maior que zero.");

        }
    }
}
