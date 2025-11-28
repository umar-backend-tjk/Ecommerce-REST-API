using Application.DTOs.UserDtos;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(DataContext context, UserManager<AppUser> userManager) : IUserRepository
{
    public async Task<int> UpdateUserAsync(AppUser user)
    {
        await userManager.UpdateAsync(user);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteUserAsync(AppUser user)
    {
        context.Users.Remove(user);
        return await context.SaveChangesAsync();
    }

    public Task<IQueryable<AppUser>> GetUsersAsync()
    {
        return Task.FromResult(context.Users.Where(u => u.IsActive).AsQueryable());
    }

    public async Task<AppUser?> GetUserByIdAsync(Guid userId)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);
    }

    public async Task<int> ChangePasswordAsync(AppUser user, ChangePasswordDto model)
    {
        var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        return result.Succeeded ? 1 : 0;
    }
}