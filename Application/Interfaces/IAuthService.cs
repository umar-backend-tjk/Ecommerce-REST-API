using Application.DTOs.Auth;
using Domain.Responses;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult> RegisterUserAsync(RegisterDto registerModel);
    Task<ServiceResult<string>> LoginAsync(LoginDto loginModel);
}