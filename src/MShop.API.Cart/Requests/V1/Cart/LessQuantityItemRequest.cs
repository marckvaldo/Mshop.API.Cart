using System.ComponentModel.DataAnnotations;

namespace Mshop.API.Cart.Requests.V1.Cart
{
    public class LessQuantityItemRequest
    {
        [Required]
        [Range(1, 100, ErrorMessage = "The quantity must be between 1 and 100.")]
        [Display(Name = "Quantity")]
        [DataType(DataType.Currency)]
        public decimal Quantity { get; set; }
    }
}
