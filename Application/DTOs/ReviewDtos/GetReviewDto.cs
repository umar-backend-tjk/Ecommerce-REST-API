namespace Application.DTOs.ReviewDtos;

public class GetReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Stars { get; set; }
    public DateTime CreatedAt { get; set; }
}