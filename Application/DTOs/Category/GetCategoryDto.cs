namespace Application.DTOs.Category;

public class GetCategoryDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; }
}