using Application.DTOs.WishList;
using Application.Interfaces;
using Domain.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishListController(IWishListService wishListService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<GetWishListDto>> GetWishList()
    {
        var result = await wishListService.GetWishListAsync();
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpPost("{productId:guid}")]
    public async Task<IActionResult> AddProductToWishList(Guid productId)
    {
        var result = await wishListService.AddProductToWishListAsync(productId);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> DeleteProductFromWishList(Guid productId)
    {
        var result = await wishListService.DeleteProductFromWishListAsync(productId);
        return StatusCode(result.StatusCode, result);
    }
}