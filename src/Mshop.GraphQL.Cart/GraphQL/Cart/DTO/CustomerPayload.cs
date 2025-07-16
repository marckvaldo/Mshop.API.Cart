namespace Mshop.API.GraphQL.Cart.GraphQL.Cart
{
    public record CustomerPayload(Guid Id, string Name, string Email, string Phone, AddressPayload Adress = null);

}
