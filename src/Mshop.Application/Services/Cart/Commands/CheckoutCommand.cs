using MediatR;

namespace Mshop.Application.Services.Cart.Commands
{
    public record CheckoutCommand(Guid CartId) : IRequest<bool>;
}
