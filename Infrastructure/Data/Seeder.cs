using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Data;

public class Seeder(
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager,
    DataContext dataContext)
{
    public async Task SeedRoles()
    {
        var newRoles = new List<IdentityRole<Guid>>()
        {
            new IdentityRole<Guid>(nameof(Roles.Customer)),
            new IdentityRole<Guid>(nameof(Roles.Manager)),
            new IdentityRole<Guid>(nameof(Roles.Admin))
        };

        var roles = await roleManager.Roles.ToListAsync();

        foreach (var role in newRoles)
        {
            if (roles.Exists(r => r.Name == role.Name)) 
                continue;
            
            await roleManager.CreateAsync(role);
        }
    }

    public async Task<bool> SeedAdmin()
    {
        var existingAdmin = await dataContext.Users.FirstOrDefaultAsync(u => u.FirstName == "Admin");

        if (existingAdmin is null)
        {
            var admin = new AppUser()
            {
                FirstName = "Admin",
                Email = "umarbackendtjk@mail.ru",
                PhoneNumber = "+992-93-566-84-40",
                UserName = "Admin"
            };
            
            var createResult = await userManager.CreateAsync(admin, "1234abcd");
            await userManager.AddToRoleAsync(admin, "Admin");

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    Log.Warning(error.Description);
                    Console.WriteLine(error.Description);
                }
                return false;
            }
        }
        
        return true;
    }
}