using MediatR;

namespace Mshop.Application.Services.Cart.Commands
{
    public record AddCustomerToCartCommand(Guid CartId, Guid CustomerId) : IRequest<bool>;
}
