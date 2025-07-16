using Mshop.Domain.Entity;
using Mshop.E2ETests.Persistence.RabbitMQ.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.E2ETests.Common.Event
{
    public class OrderCheckoutedEventMessage
    {
        public Guid Id { get; set; }
        public IEnumerable<ProductDTO> Products { get; set; }
        public IEnumerable<PaymentDTO> Payments { get; set; }
        public CustomerDTO Customer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CartStatus Status { get; set; }
    }
}
