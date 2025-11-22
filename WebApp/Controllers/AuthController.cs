using Application.DTOs.Auth;
using Application.Interfaces;
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
}