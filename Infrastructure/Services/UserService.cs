using System.Net;
using Application.DTOs.UserDtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Filters;
using Domain.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Services;

public class UserService(
    IUserRepository userRepository,
    UserManager<AppUser> userManager, 
    IMapper mapper) : IUserService
{
    public async Task<PaginationResponse<List<GetUserDto>>> GetUsersAsync(UserFilter filter)
    {
        try
        {
            var query = await userRepository.GetUsersAsync();
            
            if (!string.IsNullOrEmpty(filter.Email))
                query = query.Where(u => u.Email!.ToLower().Contains(filter.Email.ToLower()));

            if (!string.IsNullOrEmpty(filter.FirstName))
                query = query.Where(u => u.FirstName!.ToLower().Contains(filter.FirstName.ToLower()));

            if (!string.IsNullOrEmpty(filter.LastName))
                query = query.Where(u => u.LastName!.ToLower().Contains(filter.LastName.ToLower()));

            if (!string.IsNullOrEmpty(filter.PhoneNumber))
                query = query.Where(u => u.PhoneNumber!.ToLower().Contains(filter.PhoneNumber.ToLower()));

            if (filter.Role.HasValue)
            {
                var usersInRole = await userManager.GetUsersInRoleAsync(filter.Role.Value.ToString());
                var userIds = usersInRole.Select(u => u.Id).ToHashSet();
                query = query.Where(u => userIds.Contains(u.Id));
            }
            
            var totalCount = await query.CountAsync();
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            var items = await query.Skip(skip).Take(filter.PageSize).ToListAsync();

            if (!items.Any())
                return new PaginationResponse<List<GetUserDto>>(HttpStatusCode.NotFound, "Not found users");
            
            var mappedUsers = mapper.Map<List<GetUserDto>>(items);
            
            foreach (var userDto in mappedUsers)
            {
                var identityUser = await userManager.FindByIdAsync(userDto.Id.ToString());
                if (identityUser != null)
                {
                    var roles = await userManager.GetRolesAsync(identityUser);
                    userDto.Roles = roles.ToList();
                }
            }
            
            return new PaginationResponse<List<GetUserDto>>(mappedUsers, totalCount, filter.PageNumber, filter.PageSize);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in UserService.GetUsers");
            return new PaginationResponse<List<GetUserDto>>(HttpStatusCode.InternalServerError, "Unexpected error");
        }
    }

    public async Task<ServiceResult<GetUserDto>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return ServiceResult<GetUserDto>.Fail("Not found the user", HttpStatusCode.NotFound);

            var roles = await userManager.GetRolesAsync(user);
            var mappedUser = mapper.Map<GetUserDto>(user);
            mappedUser.Roles = roles.ToList();
            
            return ServiceResult<GetUserDto>.Ok(mappedUser);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in UserService.GetUserById");
            return ServiceResult<GetUserDto>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> UpdateUserAsync(UpdateUserDto model)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(model.Id);
            if (user == null)
                return ServiceResult.Fail("Not found the user", HttpStatusCode.NotFound);

            mapper.Map(model, user);

            var result = await userRepository.UpdateUserAsync(user);

            if (result == 0)
                return ServiceResult.Fail("Failed to update the user");
            
            return ServiceResult.Ok("Updated user successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in UserService.UpdateUser");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> AddRoleAsync(Guid userId, Roles role)
    {
        try
        {
            Log.Information("Trying to add a role \"{role}\" to user {uId}", role, userId);

            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                Log.Warning("Failed to add a role to user {uId}: User does not exist", userId);
                return ServiceResult.Fail("User not found", HttpStatusCode.NotFound);
            }

            var result = await userManager.AddToRoleAsync(user, role.ToString());
            if (!result.Succeeded)
            {
                Log.Warning("Failed to add a role \"{role}\" to user {userId}: {error}",
                    role, userId, result.Errors.First());
                return ServiceResult.Fail("Failed to add role");
            }

            Log.Information("Successfully added a role \"{role}\" to user {uId}", role, userId);
            return ServiceResult.Ok("Added role successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in UserService.AddRole");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }
    
    public async Task<ServiceResult> RemoveRoleFromUserAsync(Guid userId, Roles role)
    {
        try
        {
            Log.Information("Trying to remove the role \"{role}\" from user {uId}", role, userId);

            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                Log.Warning("Failed to remove the role from user {uId}: User does not exist", userId);
                return ServiceResult.Fail("User not found", HttpStatusCode.NotFound);
            }
            
            var roles = await userManager.GetRolesAsync(user);
            if (roles.Count == 1)
            {
                Log.Warning("Failed to remove the role from user {uId}: User must have at least 1 role", userId);
                return ServiceResult.Fail("User must have at least 1 role");
            }

            var result = await userManager.RemoveFromRoleAsync(user, role.ToString());
            if (!result.Succeeded)
            {
                Log.Warning("Failed to delete the role \"{role}\" from user {userId}: {error}",
                    role, userId, result.Errors.First().Description);
                return ServiceResult.Fail($"Failed to remove role: {result.Errors.First().Description}");
            }

            Log.Information("Successfully removed the role \"{role}\" from user {uId}", role, userId);
            return ServiceResult.Ok("Deleted role successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in UserService.RemoveRoleFromUser");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }
}