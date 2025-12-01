using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.OrderDtos;

public class OrderItemDto
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public Guid OrderId { get; set; }
    [Required]
    public Guid ProductId { get; set; }

    public string ProductNameSnapshot { get; set; } = null!;
    public decimal UnitPriceBase { get; set; }
    public int Quantity { get; set; }
    public decimal TotalBase { get; set; }
}