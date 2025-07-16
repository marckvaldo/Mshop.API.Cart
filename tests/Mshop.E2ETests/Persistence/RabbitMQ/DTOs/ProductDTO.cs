using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.E2ETests.Persistence.RabbitMQ.DTOs
{
    public class ProductDTO
    {
        public Guid Id { get; set; }
        public string Description { get;  set; }

        public string Name { get;  set; }

        public decimal Price { get;  set; }

        public decimal Total { get;  set; }

        public bool IsSale { get;  set; }

        public Guid CategoryId { get;  set; }

        public decimal Quantity { get;  set; }

        public string Category { get;  set; }

        public string? Thumb { get;  set; }

    }
}
