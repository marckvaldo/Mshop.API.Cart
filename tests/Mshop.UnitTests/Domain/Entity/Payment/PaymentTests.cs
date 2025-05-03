using EntityDomain = Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mshop.Domain.Entity;
using Mshop.Core.Message;

namespace Mshop.UnitTests.Domain.Entity.Payment
{
    public class PaymentTests : PaymentTestsFixture
    {
        private INotification _notification;
        public PaymentTests():base()
        { 
            _notification = new Notifications();
        }

        [Fact(DisplayName = nameof(Should_Create_Payment_With_Valid_Data))]
        [Trait("Domain", "Payment")]
        public void Should_Create_Payment_With_Valid_Data()
        {
            // Arrange
            var paymentFaker = FakerPayment(PaymentMethod.CreditCard);

            // Act
            var payment = new EntityDomain.Payment(
                amount: paymentFaker.Amount, 
                paymentMethod: paymentFaker.PaymentMethod,
                installments: paymentFaker.Installments,
                cardToken: paymentFaker.CardToken);

            payment.IsValid(_notification);

            // Assert
            Assert.NotNull(payment);
            Assert.Equal(paymentFaker.Amount, payment.Amount);
            Assert.Equal(paymentFaker.PaymentMethod, payment.PaymentMethod);
            Assert.Equal(paymentFaker.Installments, payment.Installments);
            Assert.Equal(paymentFaker.CardToken, payment.CardToken);
            Assert.Equal(paymentFaker.Status, payment.Status);
            Assert.False(_notification.HasErrors());
        }

        [Fact(DisplayName = nameof(Should_Not_Allow_Invalid_Status_Change))]
        [Trait("Domain", "Payment")]
        public void Should_Not_Allow_Invalid_Status_Change()
        {
            // Arrange
            var paymentFaker = FakerPayment(PaymentMethod.BoletoBancario);

            var payment = new EntityDomain.Payment(paymentFaker.Amount, paymentFaker.PaymentMethod, paymentFaker.BoletoNumber, paymentFaker.BoletoDueDate);
            payment.UpdateStatus(PaymentStatus.Completed);
            payment.IsValid(_notification);

            // Act
            payment.UpdateStatus(PaymentStatus.Pending);
            payment.IsValid(_notification);

            // Assert
            Assert.NotNull(payment);
            Assert.Equal(paymentFaker.Amount, payment.Amount);
            Assert.Equal(paymentFaker.PaymentMethod, payment.PaymentMethod);
            Assert.Equal(paymentFaker.Installments, payment.Installments);
            Assert.Equal(paymentFaker.CardToken, payment.CardToken);
            Assert.Equal(PaymentStatus.Completed, payment.Status);
            Assert.Equal(paymentFaker.BoletoNumber, payment.BoletoNumber);
            Assert.Equal(paymentFaker.BoletoDueDate, payment.BoletoDueDate);
            Assert.True(_notification.HasErrors());
        }

        [Fact(DisplayName = nameof(Should_Update_Payment_Status_If_Not_Completed))]
        [Trait("Domain", "Payment")]
        public void Should_Update_Payment_Status_If_Not_Completed()
        {
            // Arrange
            var paymentFaker = FakerPayment(PaymentMethod.BoletoBancario);

            var payment = new EntityDomain.Payment(paymentFaker.Amount, paymentFaker.PaymentMethod, paymentFaker.BoletoNumber, paymentFaker.BoletoDueDate);
            payment.UpdateStatus(PaymentStatus.Completed);
            payment.IsValid(_notification);

            // Act
            payment.UpdateStatus(PaymentStatus.Approved);
            payment.IsValid(_notification);

            // Assert
            Assert.NotNull(payment);
            Assert.Equal(paymentFaker.Amount, payment.Amount);
            Assert.Equal(paymentFaker.PaymentMethod, payment.PaymentMethod);
            Assert.Equal(paymentFaker.Installments, payment.Installments);
            Assert.Equal(paymentFaker.CardToken, payment.CardToken);
            Assert.Equal(PaymentStatus.Completed, payment.Status);
            Assert.Equal(paymentFaker.BoletoNumber, payment.BoletoNumber);
            Assert.Equal(paymentFaker.BoletoDueDate, payment.BoletoDueDate);
            Assert.True(_notification.HasErrors());
        }


        [Fact(DisplayName = nameof(Should_Validate_Payment_With_BoletoBancario))]
        [Trait("Domain", "Payment")]
        public void Should_Validate_Payment_With_BoletoBancario()
        {
            // Arrange
            var paymentFaker = FakerPayment(PaymentMethod.BoletoBancario);

            var payment = new EntityDomain.Payment(200.00m, paymentFaker.PaymentMethod, paymentFaker.BoletoNumber, paymentFaker.BoletoDueDate);
            // Act
            payment.IsValid(_notification);

            // Assert
            Assert.False(_notification.HasErrors());
        }

        [Fact(DisplayName = nameof(Should_Return_Invalid_When_BoletoBancario_Has_Missing_Data))]
        [Trait("Domain", "Payment")]
        public void Should_Return_Invalid_When_BoletoBancario_Has_Missing_Data()
        {
            // Arrange
            var payment = new EntityDomain.Payment(150.00m, PaymentMethod.BoletoBancario);

            // Act
            payment.IsValid(_notification);

            // Assert
            Assert.True(_notification.HasErrors());
        }

        [Fact(DisplayName = nameof(Should_Return_Invalid_When_BoletoBancario_Has_Missing_Data))]
        [Trait("Domain", "Payment")]
        public void Should_Return_Invalid_When_CreditCard_Has_Missing_Token()
        {
            // Arrange
            var paymentFaker = FakerPayment(PaymentMethod.CreditCard);

            // Act
            var payment = new EntityDomain.Payment(paymentFaker.Amount,paymentFaker.PaymentMethod);
            payment.IsValid(_notification);

            // Assert
            Assert.True(_notification.HasErrors());
        }
    }
}
