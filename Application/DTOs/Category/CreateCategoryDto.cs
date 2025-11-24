namespace Application.DTOs.Category;

public class CreateCategoryDto
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public Guid? ParentCategoryId { get; set; }
}