using Mshop.Domain.Entity;

namespace Mshop.E2ETests.GraphQL.Common
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
