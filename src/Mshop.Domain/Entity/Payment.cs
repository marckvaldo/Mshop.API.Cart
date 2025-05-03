using Mshop.Domain.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Domain.Entity
{
    public class Payment
    {
        public decimal Amount { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public PaymentStatus Status { get; private set; }
        public int? Installments { get; private set; }
        public string? CardToken { get; private set; }
        public string? BoletoNumber { get; private set; }
        public DateTime? BoletoDueDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private List<string> _notifications;

        public Payment(decimal amount, PaymentMethod paymentMethod, int? installments = null, string? cardToken = null)
        {
            Amount = amount;
            PaymentMethod = paymentMethod;
            Installments = installments;
            CardToken = cardToken;            
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            _notifications = new List<string>();
        }

        public Payment(decimal amount, PaymentMethod paymentMethod, string? boletoNumber = null, DateTime? boletoDueDate = null)
        {
            Amount = amount;
            PaymentMethod = paymentMethod;
            BoletoNumber = boletoNumber;
            BoletoDueDate = boletoDueDate;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            _notifications = new List<string>();
        }

        public Payment(decimal amount, PaymentMethod paymentMethod)
        {
            Amount = amount;
            PaymentMethod = paymentMethod;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            _notifications = new List<string>();
        }

        // Atualizar o status do pagamento
        public bool UpdateStatus(PaymentStatus newStatus)
        {
            if (Status == PaymentStatus.Completed)
            {
                _notifications.Add("Cannot change the status of a completed payment.");
                return false;
            }

            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;

            return true;
        }

        // Validar o pagamento (exemplo de comportamento de domínio)
        public bool IsValid(Core.Message.INotification notification)
        {
            var validator = new PaymentValidation();
            var validationResult = validator.Validate(this);
            validationResult.Errors.ToList().ForEach(x =>
            {
                notification.AddNotifications(x.ErrorMessage);
            });

            _notifications.ForEach(x=>
            {
                notification.AddNotifications(x);
            });

            return !notification.HasErrors();
        }

    }

    // Enum para representar os status de pagamento
    public enum PaymentStatus
    {
        Pending,     // Pagamento aguardando processamento
        Approved,    // Pagamento aprovado
        Rejected,    // Pagamento rejeitado
        Completed,   // Pagamento concluído
        Cancelled    // Pagamento cancelado
    }

    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        BoletoBancario,
        Pix,
        Other
    }
}
