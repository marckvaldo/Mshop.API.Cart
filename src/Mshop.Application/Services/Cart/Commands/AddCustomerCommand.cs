using MediatR;

namespace Mshop.Application.Services.Cart.Commands
{
    public record AddCustomerCommand(Guid CartId, Guid CustomerId) : IRequest<bool>;
}
