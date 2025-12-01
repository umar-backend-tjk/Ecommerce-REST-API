namespace Domain.Entities;

public class WishList
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt = DateTime.UtcNow;
    public List<WishListItem> Items { get; set; } = [];
}