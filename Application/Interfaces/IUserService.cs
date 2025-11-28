using Application.DTOs.UserDtos;
using Domain.Enums;
using Domain.Filters;
using Domain.Responses;

namespace Application.Interfaces;

public interface IUserService
{
    Task<PaginationResponse<List<GetUserDto>>> GetUsersAsync(UserFilter filter);
    Task<ServiceResult<GetUserDto>> GetUserByIdAsync(Guid userId);
    Task<ServiceResult> UpdateUserAsync(UpdateUserDto user);
    Task<ServiceResult> AddRoleAsync(Guid userId, Roles role);
    Task<ServiceResult> RemoveRoleFromUserAsync(Guid id, Roles role);
}