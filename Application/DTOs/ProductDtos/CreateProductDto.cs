using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ProductDtos;

public class CreateProductDto
{
    [Required] public required string Name { get; set; }
    [Required] public required string Slug { get; set; }
    [Required] public required string Description { get; set; }
    [Required] public required decimal Price { get; set; }
    public string Currency { get; set; } = "TJS";
    public string? Size { get; set; }
    public string? Color { get; set; }
    [Required] public required int StockQuantity { get; set; } = 1;
    [Required] public Guid CategoryId { get; set; }
    public bool IsFeatured { get; set; } = false;
}