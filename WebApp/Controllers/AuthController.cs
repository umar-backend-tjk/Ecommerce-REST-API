using Application.DTOs.Auth;
using Application.DTOs.UserDtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var result = await authService.RegisterUserAsync(model);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var result = await authService.LoginAsync(model);
        return StatusCode(result.StatusCode, result);
    }

    [Authorize]
    [HttpGet("account/profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await authService.GetMyProfileAsync();
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpPut("account/profile")]
    public async Task<IActionResult> UpdateProfile(UpdateUserDto model)
    {
        var result = await authService.UpdateMyProfileAsync(model);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpPut("account/change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var result = await authService.ChangePasswordAsync(dto);
        return StatusCode(result.StatusCode, result);
    }
}