using Mshop.Domain.Entity;
using Mshop.IntegrationTest.Common.Persistence.RabbitMQ.DTOs;

namespace Mshop.Domain.Event
{
    public class OrderCreateEventDTO 
    {
        public DateTime OccuredOn { get; set; }
        public Guid Id { get; set; }
        public string Version { get; set; }
        public string EventName { get; set; }
        public List<ProductDTO> Products { get; set; }
        public List<PaymentDTO> Payments { get; set; }
        public CustomerDTO Customer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CartStatus Status { get; set; }
    }

}
