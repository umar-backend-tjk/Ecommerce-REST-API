namespace Domain.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required string Currency { get; set; } = "TJS";
    public string? Size { get; set; }
    public string? Color { get; set; }
    public required int StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsFeatured { get; set; }
    
    public IList<ProductImage> Images { get; set; } = new List<ProductImage>();
}