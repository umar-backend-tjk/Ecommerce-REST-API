using Microsoft.AspNetCore.Http;

namespace Application.DTOs.BannerDtos;

public class CreateBannerDto
{
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public required IFormFile ImageFile { get; set; }
    public string? RedirectUrl { get; set; }
    public required string Position { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}