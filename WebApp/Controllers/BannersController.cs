using Application.DTOs.BannerDtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BannersController(IBannerService bannerService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateBanner(CreateBannerDto model)
    {
        var result = await bannerService.CreateBannerAsync(model);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateBanner(UpdateBannerDto model)
    {
        var result = await bannerService.UpdateBannerAsync(model);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteBanner(Guid bannerId)
    {
        var result = await bannerService.DeleteBannerAsync(bannerId);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetBanners([FromQuery] string? position)
    {
        var result = await bannerService.GetBannersAsync(position);
        return StatusCode(result.StatusCode, result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBannerById(Guid id)
    {
        var result = await bannerService.GetBannerByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }
}