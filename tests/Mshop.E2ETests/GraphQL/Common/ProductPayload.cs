namespace Mshop.E2ETests.GraphQL.Common
{
    public record ProductPayload(Guid Id, string Description, string Name, decimal Price, decimal Total, bool IsSale, Guid CategoryId, string Category, decimal Quantity, string? Thumb);
}
