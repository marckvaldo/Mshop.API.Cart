using System.ComponentModel.DataAnnotations;

namespace Mshop.API.Cart.Requests.V1.Cart
{
    public class AddCustomerToCartRequest
    {
        [Required]
        [Display(Name = "Customer Id")]
        public Guid CustomerId { get; set; }
    }
}
