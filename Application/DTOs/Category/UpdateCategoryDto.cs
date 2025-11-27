namespace Application.DTOs.Category;

public class UpdateCategoryDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; } = true;
}