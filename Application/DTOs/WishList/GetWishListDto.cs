using Application.DTOs.WishListItem;

namespace Application.DTOs.WishList;

public class GetWishListDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt = DateTime.UtcNow;
    public List<GetWishListItemDto> Items { get; set; } = [];
}