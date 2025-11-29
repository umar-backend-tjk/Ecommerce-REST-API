namespace Application.DTOs.BannerDtos;

public class GetBannerDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Subtitle { get; set; }
    public required string ImageUrl { get; set; }
    public string? RedirectUrl { get; set; }
    public required string Position { get; set; }
    public int SortOrder { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}