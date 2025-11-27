using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.ProductImage;

using Domain.Entities;

public class AddProductImageDto
{
    [Required] public IFormFile Image { get; set; } = null!;
    public bool IsMain { get; set; } = false;
}