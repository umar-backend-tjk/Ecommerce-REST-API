namespace Application.DTOs.ProductDtos;

public class UpdateProductDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public string? Currency { get; set; } = "TJS";
    public string? Size { get; set; }
    public string? Color { get; set; }
    public int? StockQuantity { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsActive { get; set; } = true;
}