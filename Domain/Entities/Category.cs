namespace Domain.Entities;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public Guid? ParentCaregoryId { get; set; }
    public int SortOrder { get; set; }
}