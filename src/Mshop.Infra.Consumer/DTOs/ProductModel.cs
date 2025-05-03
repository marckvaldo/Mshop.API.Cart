namespace Mshop.Infra.Consumer.DTOs
{
    public record ProductModel(Guid Id, string Description, string Name, decimal Price, bool IsPromotion, Guid CategoryId, string Category, string? Thumb);
}

