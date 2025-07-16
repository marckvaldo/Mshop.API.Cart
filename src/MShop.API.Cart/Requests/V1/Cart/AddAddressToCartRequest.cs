using System.ComponentModel.DataAnnotations;

namespace Mshop.API.Cart.Requests.V1.Cart
{
    public class AddAddressToCartRequest
    {
        [Required]
        [StringLength(100)]
        public string Street { get; set; }

        [Required]
        [StringLength(10)]
        public string Number { get; set; }

        [StringLength(100)]
        public string Complement { get; set; }

        [Required]
        [StringLength(100)]
        public string Neighborhood { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Use a sigla do estado, ex: 'PE'")]
        public string State { get; set; }

        [Required]
        [RegularExpression(@"^\d{5}-?\d{3}$", ErrorMessage = "CEP inválido")]
        public string PostalCode { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }
    }
}
