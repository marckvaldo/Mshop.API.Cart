using Mshop.Infra.Consumer.DTOs;

namespace Mshop.Application.Interface
{
    public interface ICustomerConsumer
    {
        Task<CustomerModel?> GetCustomerByIdAsync(Guid customerId);
    }
}
