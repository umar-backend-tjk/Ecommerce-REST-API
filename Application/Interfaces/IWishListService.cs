using Application.DTOs.WishList;
using Domain.Responses;

namespace Application.Interfaces;

public interface IWishListService
{
    Task<ServiceResult<GetWishListDto>> GetWishListAsync();
    Task<ServiceResult> AddProductToWishListAsync(Guid productId);
    Task<ServiceResult> DeleteProductFromWishListAsync(Guid productId);
}