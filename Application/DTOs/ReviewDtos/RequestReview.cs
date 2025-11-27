using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ReviewDtos;

public class RequestReview
{
    [Required] public Guid ProductId { get; set; }
    [Range(1, 5)] public int Stars { get; set; }
}