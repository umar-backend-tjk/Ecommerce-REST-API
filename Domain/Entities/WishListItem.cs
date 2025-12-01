namespace Domain.Entities;

public class WishListItem
{
    public Guid Id { get; set; }
    public Guid WishListId { get; set; }
    public WishList? WishList { get; set; }
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public DateTime CreatedAt = DateTime.UtcNow;
}