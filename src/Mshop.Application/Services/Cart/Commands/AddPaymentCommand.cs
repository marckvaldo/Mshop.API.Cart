using MediatR;
using Mshop.Application.Commons.DTO;

namespace Mshop.Application.Services.Cart.Commands
{
    public record AddPaymentCommand(Guid CartId, PaymentDTO Payment) : IRequest<bool>
    {
    }
}
