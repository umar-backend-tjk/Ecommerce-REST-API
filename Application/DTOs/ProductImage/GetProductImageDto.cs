namespace Application.DTOs.ProductImage;

public class GetProductImageDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public required string ImageUrl { get; set; }
    public bool IsMain { get; set; } = false;
    public int SortOrder { get; set; }
}