using Domain.Enums;

namespace Domain.Filters;

public class UserFilter : BaseFilter
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public Roles? Role { get; set; }
}