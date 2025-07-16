namespace Mshop.API.GraphQL.Cart.GraphQL.Cart.DTO
{
    public class CartPayload
    {
        public Guid Id { get; set; }

        public IEnumerable<ProductPayload> Products { get; set; }

        public CustomerPayload Customer { get; set; }

        public IEnumerable<PaymentPayload> Payments { get; set; }

        public CartPayload(Guid id, IEnumerable<ProductPayload> products, CustomerPayload customer, IEnumerable<PaymentPayload> payments)
        {
            Id = id;
            Products = products;
            Customer = customer;
            Payments = payments;
        }

    }
}

