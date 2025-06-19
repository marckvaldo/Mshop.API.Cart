using MediatR;
using Mshop.Application.Commons.Response;
using Mshop.Core.DomainObject;

namespace Mshop.Application.Services.Cart.Commands
{
    public record AddItemToCartCommand(
        Guid CartId,
        Guid ProductId,
        int Quantity) : IRequest<Result<CartResponse>>;
}
