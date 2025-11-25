namespace Domain.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public int SortOrder { get; set; }
    public IList<Category> ChildrenCategories { get; set; } = new List<Category>();
}