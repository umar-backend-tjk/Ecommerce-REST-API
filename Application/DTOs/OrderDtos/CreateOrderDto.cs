using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.OrderDtos;

public class CreateOrderDto
{
    [Required] 
    public string FullName { get; set; } = null!;

    [Required] 
    public string Phone { get; set; } = null!;

    [Required] 
    public string Country { get; set; } = null!;

    [Required]
    public string City { get; set; } = null!;

    [Required] 
    public string AddressLine { get; set; } = null!;

    public string? PostalCode { get; set; }

    [Required] 
    public string Currency { get; set; } = null!;

    [Required] 
    public decimal CurrencyRate { get; set; }
}