using Application.DTOs.UserDtos;
using Domain.Entities;
using Domain.Responses;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<int> UpdateUserAsync(AppUser user);
    Task<int> DeleteUserAsync(AppUser user);
    Task<List<AppUser>> GetUsersAsync();
    Task<AppUser?> GetUserByIdAsync(Guid userId);
    Task<int> ChangePasswordAsync(AppUser user, ChangePasswordDto changePasswordDto);
}