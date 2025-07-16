using Mshop.Domain.Entity;
using System.ComponentModel.DataAnnotations;

namespace Mshop.API.Cart.Requests.V1.Cart
{
    public class AddPaymentToCartRequest
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Range(1, 36, ErrorMessage = "Número de parcelas deve ser entre 1 e 36.")]
        public int? Installments { get; set; }

        [StringLength(200)]
        public string? CardToken { get; set; }

        [StringLength(100)]
        public string? BoletoNumber { get; set; }

        public DateTime? BoletoDueDate { get; set; }
    }
}
