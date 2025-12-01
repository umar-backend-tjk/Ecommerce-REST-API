namespace Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }

    public string ProductNameSnapshot { get; set; } = null!;
    public decimal UnitPriceBase { get; set; }
    public int Quantity { get; set; }
    public decimal TotalBase { get; set; }

    public Order Order { get; set; } = null!;
}