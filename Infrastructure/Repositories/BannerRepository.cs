using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BannerRepository(DataContext context) : IBannerRepository
{
    public async Task<int> CreateBannerAsync(Banner banner)
    {
        await context.Banners.AddAsync(banner);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateBannerAsync(Banner banner)
    {
        context.Banners.Update(banner);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteBannerAsync(Banner banner)
    {
        context.Banners.Remove(banner);
        return await context.SaveChangesAsync();
    }

    public async Task<List<Banner>> GetBannersAsync(string? position)
    {
        return position != null
            ? await context.Banners
                .AsNoTracking()
                .Where(b => b.IsActive && b.Position == position)
                .OrderBy(b => b.SortOrder)
                .ToListAsync()
            : await context.Banners
                .AsNoTracking()
                .Where(b => b.IsActive)
                .OrderBy(b => b.SortOrder)
                .ToListAsync();
    }

    public async Task<Banner?> GetBannerByIdAsync(Guid bannerId)
    {
        return await context.Banners.FirstOrDefaultAsync(b => b.Id == bannerId && b.IsActive);
    }

    public async Task<Banner?> GetBannerByPositionAsync(string position)
    {
        return await context.Banners
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Position == position);
    }

    public async Task<int> GetNextSortOrderAsync(string position)
    {
        var maxSortOrder = await context.Banners
            .Where(p => p.Position == position)
            .MaxAsync(c => (int?)c.SortOrder);

        return (maxSortOrder ?? 0) + 1;
    }
}