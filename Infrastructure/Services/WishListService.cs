using System.Security.Claims;
using Application.DTOs.WishList;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Responses;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Infrastructure.Services;

public class WishListService(
    IWishListRepository repository, 
    IMapper mapper,
    IHttpContextAccessor accessor) : IWishListService
{
    public async Task<ServiceResult<List<GetWishListDto>>> GetWishListAsync(Guid userId)
    {
        try
        {
            var wishList = await repository.GetWishListAsync(userId);

            var mappedList = mapper.Map<List<GetWishListDto>>(wishList.Items);

            return ServiceResult<List<GetWishListDto>>.Ok(mappedList);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error in WishListService.GetWishListAsync");
            return ServiceResult<List<GetWishListDto>>.Fail("Unexpected error");
        }
    }

    public async Task<ServiceResult> AddProductToWishListAsync(Guid productId)
    {
        try
        {
            var userIdStr = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var userId = Guid.Parse(userIdStr);

            var wishList = await repository.GetWishListAsync(userId);

            var item = new WishListItem
            {
                Id = Guid.NewGuid(),
                WishListId = wishList.Id,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow
            };

            await repository.CreateWishListItemAsync(item);

            Log.Information("Added the product {ProductId} to wish-list of user {UserId}",
                productId, userId);

            return ServiceResult.Ok("Added product to wish-list");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error in WishListService.AddProductToWishList");
            return ServiceResult.Fail("Unexpected error");
        }
    }

    public async Task<ServiceResult> DeleteProductFromWishListAsync(Guid productId)
    {
        try
        {
            var userIdStr = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var userId = Guid.Parse(userIdStr);

            var wishList = await repository.GetWishListAsync(userId);

            var item = wishList.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
                return ServiceResult.Fail("Not found the product in wish-list");

            await repository.DeleteWishListItemAsync(item);

            Log.Information("Deleted product {ProductId} from wish-list of user {UserId}",
                productId, userId);

            return ServiceResult.Ok("Deleted product from wish-list");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error in WishListService.DeleteProductFromWishListAsync()");
            return ServiceResult.Fail("Unexpected error");
        }
    }
}
