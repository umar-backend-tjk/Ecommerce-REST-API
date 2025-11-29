namespace Domain.Entities;

public class Cart(Guid userId)
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; } = userId;
    public AppUser? User { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<CartItem> Items = [];
}