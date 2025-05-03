using Mshop.Infra.Consumer.DTOs;

namespace Mshop.Infra.Consumer.GRPC
{
    public interface IServicerGRPC
    {
        Task<CustomerModel?> GetCustomerByIdAsync(Guid customerId);

        Task<ProductModel?> GetProductByIdAsync(Guid productId);
    }
}
