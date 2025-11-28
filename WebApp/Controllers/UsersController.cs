using Application.DTOs.UserDtos;
using Application.Interfaces;
using Domain.Enums;
using Domain.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserService userService) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserFilter filter)
    {
        var result = await userService.GetUsersAsync(filter);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var result = await userService.GetUserByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UpdateUserDto model)
    {
        var result = await userService.UpdateUserAsync(model);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/add-role")]
    public async Task<IActionResult> AddRoleToUser(Guid id, Roles role)
    {
        var result = await userService.AddRoleAsync(id, role);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}/delete-role")]
    public async Task<IActionResult> RemoveRoleFromUser(Guid id, Roles role)
    {
        var result = await userService.RemoveRoleFromUserAsync(id, role);
        return StatusCode(result.StatusCode, result);
    }
}