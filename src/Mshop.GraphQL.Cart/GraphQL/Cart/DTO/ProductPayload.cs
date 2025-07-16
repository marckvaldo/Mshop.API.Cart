namespace Mshop.API.GraphQL.Cart.GraphQL.Cart
{
    public record ProductPayload(Guid Id, string Description, string Name, decimal Price, decimal Total, bool IsSale, Guid CategoryId, string Category, decimal Quantity, string? Thumb);
}
