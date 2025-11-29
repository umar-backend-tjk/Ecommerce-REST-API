using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.CartItem;

public class UpdateCartItemDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [Range(0, 100, ErrorMessage = "Quantity must be between 0 and 100")]
    public int Quantity { get; set; }
}