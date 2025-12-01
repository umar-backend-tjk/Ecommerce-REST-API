using Domain.Entities;

namespace Application.Interfaces;

public interface IWishListRepository
{
    Task<int> CreateWishListAsync(WishList wishList);
    Task<WishList> GetWishListAsync(Guid userId);
    Task<int> CreateWishListItemAsync(WishListItem wishListItem);
    Task<int> DeleteWishListItemAsync(WishListItem wishListItem);
}