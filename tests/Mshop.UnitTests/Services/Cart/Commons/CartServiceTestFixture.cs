using Mshop.Application.Commons.DTO;
using Mshop.Core.Test.Domain;
using Mshop.Domain.Entity;

namespace Mshop.UnitTests.Services.Cart.Commons
{
    public class CartServiceTestFixture : DomainEntityFixture
    {
        public CartServiceTestFixture() : base()
        {
            
        }

        public PaymentDTO PaymentToPaymamentDTO(Payment payment)
        {
            return new PaymentDTO(
                    payment.Amount,
                    payment.PaymentMethod,
                    payment.Status,
                    payment.Installments,
                    payment.CardToken,
                    payment.BoletoNumber,
                    payment.BoletoDueDate);


        }
    }
}
