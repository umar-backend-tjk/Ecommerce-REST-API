namespace Application.DTOs.CartItem;

public class AddCartItemDto
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}