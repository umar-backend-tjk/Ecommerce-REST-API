using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserDtos;

public class UpdateUserDto
{
    public Guid Id { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}