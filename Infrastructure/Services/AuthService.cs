using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Application.DTOs.Auth;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Responses;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Infrastructure.Services;

public class AuthService(
    UserManager<AppUser> userManager,
    IMapper mapper,
    IConfiguration configuration) : IAuthService
{
    public async Task<ServiceResult> RegisterUserAsync(RegisterDto model)
    {
        try
        {
            Log.Information("Trying to register user {login}", model.EmailOrPhoneNumber);

            var login = model.EmailOrPhoneNumber;
            var isEmail = new EmailAddressAttribute().IsValid(login);
            
            var existingUser = await userManager.Users
                .FirstOrDefaultAsync(u => u.Email == login || u.PhoneNumber == login);

            if (existingUser != null)
            {
                var field = existingUser.Email == login ? "email" : "phone number";
                Log.Warning("User with {field} '{login}' already exists.", field, login);

                return ServiceResult.Fail("User already exists");
            }
            
            var user = mapper.Map<AppUser>(model);
            if (isEmail)
                user.Email = login;
            else
                user.PhoneNumber = login;

            user.UserName = login;

            var createResult = await userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                var error = createResult.Errors.First().Description;
                Log.Warning("Failed to register user {login}: {error}", login, error);
                return ServiceResult.Fail(error);
            }
            
            var roleResult = await userManager.AddToRoleAsync(user, Roles.Customer.ToString());
            if (!roleResult.Succeeded)
            {
                Log.Warning("Failed to add role Customer to {login}: {error}", login, roleResult.Errors.First().Description);
            }

            Log.Information("User {login} registered successfully", login);
            return ServiceResult.Ok("User registered successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error in RegisterUserAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }


    public async Task<ServiceResult<string>> LoginAsync(LoginDto model)
    {
        try
        {
            Log.Information("User {login} tries to log in", model.EmailOrPhoneNumber);
            
            var existingUser = await userManager.Users
                .FirstOrDefaultAsync(u => u.Email == model.EmailOrPhoneNumber || u.PhoneNumber == model.EmailOrPhoneNumber);

            if (existingUser is null)
            {
                Log.Warning("Not found the {emailOrPhoneNumber}", model.EmailOrPhoneNumber);
                return ServiceResult<string>.Fail("Invalid email/phone-number or password");
            }

            if (!await userManager.CheckPasswordAsync(existingUser, model.Password))
            {
                Log.Warning("Invalid password entered");
                return ServiceResult<string>.Fail("Invalid email or phone-number or password");
            }

            existingUser.LastLoginAt = DateTime.UtcNow;
            await userManager.UpdateAsync(existingUser);
        
            var jwtToken = await GenerateJwtToken(existingUser);
            Log.Information("User {email} logged in", model.EmailOrPhoneNumber);
            return ServiceResult<string>.Ok(jwtToken);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in LoginAsync");
            return ServiceResult<string>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }
    
    private async Task<string> GenerateJwtToken(AppUser user)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        var security = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(security, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim("first-name", user.FirstName ?? ""),
            new Claim("last-name", user.LastName ?? ""),
            new Claim("phone-number", user.PhoneNumber ?? ""),
        };

        var userRoles = await userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

        var tokenDescription = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(configuration["Jwt:ExpiryMinutes"]!)),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescription);
        return tokenString;
    }
}