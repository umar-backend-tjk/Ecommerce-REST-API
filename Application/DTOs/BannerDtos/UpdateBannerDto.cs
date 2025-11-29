using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.BannerDtos;

public class UpdateBannerDto
{
    [Required]
    public required Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public IFormFile? ImageFile { get; set; }
    public string? RedirectUrl { get; set; }
    public string? Position { get; set; }
    public int? SortOrder { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}