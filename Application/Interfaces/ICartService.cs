using Application.DTOs.Cart;
using Application.DTOs.CartItem;
using Domain.Responses;

namespace Application.Interfaces;

public interface ICartService
{
    Task<ServiceResult> DeleteCartAsync(Guid cartId);
    Task<ServiceResult<GetCartDto>> GetCartAsync();
    Task<ServiceResult> AddCartItemAsync(AddCartItemDto cartItemDto);
    Task<ServiceResult> UpdateCartItemAsync(UpdateCartItemDto cartItemDto);
    Task<ServiceResult> DeleteCartItemAsync(Guid cartItemId);
}