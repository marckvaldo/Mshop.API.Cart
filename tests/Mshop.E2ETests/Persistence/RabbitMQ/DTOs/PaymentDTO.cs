using Mshop.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.E2ETests.Persistence.RabbitMQ.DTOs
{
    public class PaymentDTO
    {
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get;  set; }
        public PaymentStatus Status { get;  set; }
        public int? Installments { get;  set; }
        public string? CardToken { get;  set; }
        public string? BoletoNumber { get;  set; }
        public DateTime? BoletoDueDate { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public DateTime UpdatedAt { get;  set; }
    }
}
