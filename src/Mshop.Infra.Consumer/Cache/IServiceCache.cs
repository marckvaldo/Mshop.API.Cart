using Mshop.Infra.Consumer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Infra.Consumer.Cache
{
    public interface IServiceCache
    {
        Task<ProductModel> GetProductById(Guid id);
    }
}
