namespace Application.DTOs.WishListItem;

public class GetWishListItemDto
{
    public Guid Id { get; set; }
    public Guid WishListId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime CreatedAt = DateTime.UtcNow;
}