using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.E2ETests.Persistence.DTOs
{
    public class ProductsPersistenceDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Stock { get; set; }
        public bool IsActive { get; set; }
        public bool IsSale { get; set; }
        public Guid CategoryId { get; set; }
        public string Thumb { get; set; }
    }
}
