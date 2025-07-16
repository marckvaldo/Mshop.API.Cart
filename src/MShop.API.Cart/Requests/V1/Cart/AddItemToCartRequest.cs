using System.ComponentModel.DataAnnotations;

namespace Mshop.API.Cart.Requests.V1.Cart
{
    public class AddItemToCartRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Range(1, 100)]
        public int Quantity { get; set; }        
    }
}
