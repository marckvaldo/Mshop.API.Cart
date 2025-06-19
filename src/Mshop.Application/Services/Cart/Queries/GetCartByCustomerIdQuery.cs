using MediatR;
using Mshop.Application.Commons.Response;
using Mshop.Core.DomainObject;

namespace Mshop.Application.Services.Cart.Queries
{
    public record class GetCartByCustomerIdQuery(Guid CustomerId) : IRequest<Result<CartResponse>>;
}
