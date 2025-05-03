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
        Task<Result<CartDTO>> AddItemToCart(Guid cartId, Guid productId, int quantity);
        Task<bool> RemoveItemFromCart(Guid cartId, Guid productId);
        Task<bool> RemoveQuantityFromCart(Guid cartId, Guid productId);
        Task<bool> ClearCart(Guid cartId);
        Task<bool> AddCustomer(Guid cartId, Guid CustomerId);
        Task<bool> AddAddressToCart(Guid cartId, AddressDTO address);
        Task<CartDTO> GetCartDetails(Guid cartId);
        Task<CartDTO> GetCartByCustomer(Guid customerId);
    }
}
