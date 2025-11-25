namespace Application.DTOs.Product;

public class GetProductDto
{
    public Guid Id { get; set; }
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
    public DateTime CreatedAt { get; set; }
}