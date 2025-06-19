using Mshop.Application.Commons.DTO;
using Mshop.Application.Commons.Response;
using Mshop.Core.DomainObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mshop.Application.Interface
{
    public interface ICartServices
    {
        Task<Result<CartResponse>> AddItemToCartAsync(Guid cartId, Guid productId, int quantity, CancellationToken cancellationToken);
        Task<bool> RemoveItemFromCart(Guid cartId, Guid productId);
        Task<bool> RemoveQuantityFromCart(Guid cartId, Guid productId);
        Task<bool> ClearCart(Guid cartId);
        Task<bool> AddCustomer(Guid cartId, Guid CustomerId);
        Task<bool> AddAddressToCart(Guid cartId, AddressDTO address);
        Task<CartResponse> GetCartDetails(Guid cartId, CancellationToken cancellationToken);
        Task<CartResponse> GetCartByCustomerId(Guid customerId, CancellationToken cancellationToken);
    }
}
