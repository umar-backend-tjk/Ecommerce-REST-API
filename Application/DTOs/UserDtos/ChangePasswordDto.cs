using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.UserDtos;

public class ChangePasswordDto
{
    [Required]
    public required string CurrentPassword { get; set; }

    [Required]
    [MinLength(4)]
    public required string NewPassword { get; set; }

    [Required]
    [Compare("NewPassword", ErrorMessage = "Пароли не совпадают.")]
    public required string ConfirmPassword { get; set; }
}
