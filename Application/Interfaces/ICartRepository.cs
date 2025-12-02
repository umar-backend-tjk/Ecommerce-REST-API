using Domain.Entities;

namespace Application.Interfaces;

public interface ICartRepository
{
    Task CreateCartAsync(Guid userId);
    Task<int> DeleteCartAsync(Cart cart);
    Task<Cart?> GetCartAsync(Guid userId);
    Task<Cart?> GetCartByIdAsync(Guid cartId);
    Task<int> CreateCartItemAsync(CartItem cartItem);
    Task<int> UpdateCartItemAsync(CartItem cartItem);
    Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId);
    Task<int> DeleteCartItemAsync(CartItem cartItem);
    Task<int> SaveChangesAsync();
}