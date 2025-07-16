using Mshop.Domain.Entity;

namespace Mshop.Application.Commons.DTO
{
    public record PaymentDTO(
        decimal Amount,
        PaymentMethod PaymentMethod,
        PaymentStatus PaymentStatus,
        int? Installments,
        string? CardToken,
        string? BoletoNumber,
        DateTime? BoletoDueDate
    );
}
