using Application.DTOs.CartItem;

namespace Application.DTOs.Cart;

public class GetCartDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public List<GetCartItemDto> Items { get; set; } = [];
}