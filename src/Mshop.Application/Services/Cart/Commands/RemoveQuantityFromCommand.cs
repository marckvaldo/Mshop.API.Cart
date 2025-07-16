using MediatR;

namespace Mshop.Application.Services.Cart.Commands
{
    public record class RemoveQuantityFromCommand(
        Guid CartId,
        Guid ProductId,
        decimal Quantity) : IRequest<bool>
    {
    }
}
