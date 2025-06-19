using MediatR;
using Mshop.Application.Commons.Response;
using Mshop.Core.DomainObject;

namespace Mshop.Application.Services.Cart.Queries
{
    public record class GetCartDetailsQuery(Guid CartId) : IRequest<Result<CartResponse>>;
}
