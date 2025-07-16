namespace Mshop.E2ETests.Common
{
    public record ProductModelGrpc(Guid Id, string Description, string Name, decimal Price, bool IsPromotion, Guid CategoryId, string Category, string? Thumb);
}
