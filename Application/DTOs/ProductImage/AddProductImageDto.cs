using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.ProductImage;

using Domain.Entities;

public class AddProductImageDto
{
    [Required] public Guid ProductId { get; set; }
    public Product? Product { get; set; }

    [Required] public IFormFile Image { get; set; } = null!;
    public bool IsMain { get; set; } = false;
}