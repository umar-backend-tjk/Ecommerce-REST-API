using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public class RegisterDto
{
    [Required]
    public required string FirstName { get; set; }
    [Required]
    public required string EmailOrPhoneNumber { get; set; }
    [Required]
    public required string Password { get; set; }
}