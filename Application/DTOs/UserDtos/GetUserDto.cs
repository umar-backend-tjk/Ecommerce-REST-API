namespace Application.DTOs.UserDtos;

public class GetUserDto
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; }
}