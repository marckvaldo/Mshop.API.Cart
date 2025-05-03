using Mshop.Infra.Consumer.DTOs;

namespace Mshop.Application.Interface
{
    public interface IProductConsumer
    {
        Task<ProductModel?> GetProductByIdAsync(Guid productId);
    }
}
