using Application.DTOs.WishList;
using Domain.Responses;

namespace Application.Interfaces;

public interface IWishListService
{
    Task<ServiceResult<List<GetWishListDto>>> GetWishListAsync(Guid userId);
    Task<ServiceResult> AddProductToWishListAsync(Guid productId);
    Task<ServiceResult> DeleteProductFromWishListAsync(Guid productId);
}