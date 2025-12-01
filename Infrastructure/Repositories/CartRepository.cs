using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CartRepository(DataContext context) : ICartRepository
{
    public async Task<int> CreateCartAsync(Cart cart)
    {
        await context.Carts.AddAsync(cart);
        return await context.SaveChangesAsync();
    }
    

    public async Task<int> DeleteCartAsync(Cart cart)
    {
        context.Carts.Remove(cart);
        return await context.SaveChangesAsync();
    }

    public async Task<Cart?> GetCartAsync(Guid userId)
    {
        return await context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart?> GetCartByIdAsync(Guid cartId)
    {
        return await context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == cartId);
    }

    public async Task<int> CreateCartItemAsync(CartItem cartItem)
    {
        await context.CartItems.AddAsync(cartItem);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateCartItemAsync(CartItem cartItem)
    { 
        context.CartItems.Update(cartItem);
        return await context.SaveChangesAsync();
    }

    public async Task<CartItem?> GetCartItemByIdAsync(Guid cartItemId)
    {
        return await context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId);
    }

    public async Task<int> DeleteCartItemAsync(CartItem cartItem)
    {
        context.CartItems.Remove(cartItem);
        return await context.SaveChangesAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}