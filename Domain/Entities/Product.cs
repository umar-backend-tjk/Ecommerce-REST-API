namespace Domain.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Description { get; set; }
    public decimal? Price { get; set; }
    public required string Currency { get; set; } = "TJS";
    public int Rating { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public int? StockQuantity { get; set; }
    public Guid? CategoryId { get; set; }
    public Category? Category { get; set; }
    public bool IsFeatured { get; set; }
    
    public IList<ProductImage> Images { get; set; } = new List<ProductImage>();
    public IList<Review> Reviews { get; set; } = new List<Review>();
}