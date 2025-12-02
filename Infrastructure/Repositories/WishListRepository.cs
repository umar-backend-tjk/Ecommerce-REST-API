using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class WishListRepository(DataContext context) : IWishListRepository
{
    public async Task CreateWishListAsync(Guid userId)
    {
        var wishList = new WishList
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            Items = []
        };
        await context.WishLists.AddAsync(wishList);
        await context.SaveChangesAsync();
    }

    public async Task<WishList> GetWishListAsync(Guid userId)
    {
        return await context.WishLists
            .Include(w => w.Items)
            .FirstAsync(w => w.UserId == userId);
    }

    public async Task<int> CreateWishListItemAsync(WishListItem wishListItem)
    {
        await context.WishListItems.AddAsync(wishListItem);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteWishListItemAsync(WishListItem wishListItem)
    {
        context.WishListItems.Remove(wishListItem);
        return await context.SaveChangesAsync();
    }
}