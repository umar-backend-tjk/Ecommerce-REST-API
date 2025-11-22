using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Auth;

public class LoginDto
{
    [Required]
    public required string EmailOrPhoneNumber { get; set; }
    [Required]
    public required string Password { get; set; }
}