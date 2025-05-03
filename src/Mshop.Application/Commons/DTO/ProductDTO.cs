using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Application.Commons.DTO
{
    public record ProductDTO(Guid Id, string Description, string Name, decimal Price, decimal Total, bool IsSale, Guid CategoryId, string Category, decimal Quantity, string? Thumb);
}

