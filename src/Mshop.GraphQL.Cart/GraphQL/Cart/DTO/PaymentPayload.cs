using Mshop.Domain.Entity;

namespace Mshop.API.GraphQL.Cart.GraphQL.Cart
{
    public record PaymentPayload(
        decimal Amount,
        string PaymentMethod,
        string PaymentStatus,
        int? Installments,
        string? CardToken,
        string? BoletoNumber,
        DateTime? BoletoDueDate
    );
}
