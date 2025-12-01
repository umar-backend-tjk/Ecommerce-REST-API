using Domain.Enums;

namespace Domain.Entities;

using System;
using System.Collections.Generic;

public class Order
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = default!;
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; }

    public decimal TotalAmountBase { get; set; }
    public string Currency { get; set; } = default!;
    public decimal CurrencyRate { get; set; }

    public string FullName { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Country { get; set; } = default!;
    public string City { get; set; } = default!;
    public string AddressLine { get; set; } = default!;
    public string? PostalCode { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CanceledAt { get; set; }

    public List<OrderItem> Items { get; set; } = [];
}