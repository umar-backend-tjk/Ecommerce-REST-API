using Application.DTOs.CartItem;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController(ICartService cartService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCart()
    {
        var result = await cartService.CreateCartAsync();
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var result = await cartService.GetCartAsync();
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpPost("items")]
    public async Task<IActionResult> AddCartItem(AddCartItemDto model)
    {
        var result = await cartService.AddCartItemAsync(model);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpPut("items")]
    public async Task<IActionResult> UpdateCartItem(UpdateCartItemDto model)
    {
        var result = await cartService.UpdateCartItemAsync(model);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> DeleteCartItem(Guid itemId)
    {
        var result = await cartService.DeleteCartItemAsync(itemId);
        return StatusCode(result.StatusCode, result);
    }
}