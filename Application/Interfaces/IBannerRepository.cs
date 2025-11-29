using Domain.Entities;

namespace Application.Interfaces;

public interface IBannerRepository
{
    Task<int> CreateBannerAsync(Banner banner);
    Task<int> UpdateBannerAsync(Banner banner);
    Task<int> DeleteBannerAsync(Banner banner);
    Task<List<Banner>> GetBannersAsync(string? position);
    Task<Banner?> GetBannerByIdAsync(Guid bannerId);
    Task<Banner?> GetBannerByPositionAsync(string position);
    Task<int> GetNextSortOrderAsync(string position);
}