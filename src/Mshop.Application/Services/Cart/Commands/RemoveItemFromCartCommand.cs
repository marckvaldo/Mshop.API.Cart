using MediatR;
using Mshop.Application.Commons.Response;
using Mshop.Core.DomainObject;

namespace Mshop.Application.Services.Cart.Commands
{
    public record class RemoveItemFromCartCommand(
        Guid CartId,
        Guid ProductId) : IRequest<bool>
    {
    }
}
