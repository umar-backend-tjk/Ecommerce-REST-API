using Application.DTOs.ProductImage;

namespace Application.DTOs.ProductDtos;

using Domain.Entities;

public class GetProductDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public required string Description { get; set; }
    public int? Rating { get; set; }
    public decimal? Price { get; set; }
    public required string Currency { get; set; } = "TJS";
    public string? Size { get; set; }
    public string? Color { get; set; }
    public int? StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<GetProductImageDto> Images { get; set; } = [];
}