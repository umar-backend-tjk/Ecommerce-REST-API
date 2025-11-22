using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public override Guid Id { get; set; }
    public override string? Email { get; set; }
    public override string? UserName { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public override string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
}