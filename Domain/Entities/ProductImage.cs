namespace Domain.Entities;

public class ProductImage
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; }
    public required string ImageUrl { get; set; }
    public bool IsMain { get; set; }
    public int SortOrder { get; set; }
}