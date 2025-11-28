using Application.DTOs.UserDtos;
using Domain.Filters;
using Domain.Responses;

namespace Application.Interfaces;

public interface IUserService
{
    Task<ServiceResult<List<GetUserDto>>> GetUsersAsync(UserFilter filter);
    Task<ServiceResult<GetUserDto>> GetUserByIdAsync(Guid userId);
    Task<ServiceResult> UpdateUserAsync(UpdateUserDto user);
}