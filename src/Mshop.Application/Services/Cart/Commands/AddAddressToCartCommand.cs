using MediatR;
using Mshop.Application.Commons.DTO;

namespace Mshop.Application.Services.Cart.Commands
{
    public record AddAddressToCartCommand(Guid CartId, AddressDTO Address) : IRequest<bool>;
}
