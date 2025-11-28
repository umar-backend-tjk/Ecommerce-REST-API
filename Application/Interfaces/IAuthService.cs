using Application.DTOs.Auth;
using Application.DTOs.UserDtos;
using Domain.Entities;
using Domain.Responses;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult> RegisterUserAsync(RegisterDto registerModel);
    Task<ServiceResult<string>> LoginAsync(LoginDto loginModel);
    Task<ServiceResult<GetUserDto>> GetMyProfileAsync();
    Task<ServiceResult> UpdateMyProfileAsync(UpdateUserDto model);
    Task<ServiceResult> ChangePasswordAsync(ChangePasswordDto dto);
}