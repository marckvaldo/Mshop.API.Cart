using Mshop.Application.Commons.DTO;
using Mshop.Core.Test.Domain;
using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.UnitTests.Services
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
                    payment.BoletoDueDate,
                    payment.CreatedAt,
                    payment.UpdatedAt);


        }
    }
}
