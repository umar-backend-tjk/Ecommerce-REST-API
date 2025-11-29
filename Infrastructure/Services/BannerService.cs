using System.Net;
using Application.DTOs.BannerDtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Responses;
using Serilog;

namespace Infrastructure.Services;

public class BannerService(
    IBannerRepository bannerRepository,
    IFileStorageService fileService,
    ICacheService cacheService,
    IMapper mapper) : IBannerService
{
    public async Task<ServiceResult> CreateBannerAsync(CreateBannerDto model)
    {
        try
        {
            Log.Information("Trying to create banner in position {position}", model.Position);
            var banner = mapper.Map<Banner>(model);

            var cacheKey = $"Banner-{model.Position}";
            
            var ext = Path.GetExtension(model.ImageFile.FileName);

            if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                banner.ImageUrl = await fileService.SaveFileAsync(model.ImageFile, "images/banners/");
                await cacheService.RemoveAsync(cacheKey);
            }
            else
            {
                return ServiceResult.Fail("Not supported image type");
            }

            banner.SortOrder = await bannerRepository.GetNextSortOrderAsync(model.Position);
            
            var result = await bannerRepository.CreateBannerAsync(banner);

            if (result == 0)
            {
                Log.Warning("Failed to create a banner in position {position}", model.Position);
                return ServiceResult.Fail("Failed to create banner");
            }
            
            Log.Information("Created a banner in position {position}", model.Position);
            return ServiceResult.Ok("Banner created successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in BannerService.CreateBannerAsync()");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> UpdateBannerAsync(UpdateBannerDto model)
    {
        try
        {
            Log.Information("Trying to update banner with id {bId}", model.Id);

            var banner = await bannerRepository.GetBannerByIdAsync(model.Id);
            if (banner == null)
            {
                Log.Warning("Failed to update banner: Not found the banner");
                return ServiceResult.Fail("Not found the banner", HttpStatusCode.NotFound);
            }

            var oldPosition = banner.Position;

            if (model.ImageFile != null)
            {
                var ext = Path.GetExtension(model.ImageFile.FileName);

                if (ext.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
                    ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                    ext.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    banner.ImageUrl = await fileService.SaveFileAsync(model.ImageFile, "images/banners/");
                }
                else
                {
                    return ServiceResult.Fail("Not supported image type");
                }
            }

            mapper.Map(model, banner);

            var result = await bannerRepository.UpdateBannerAsync(banner);
            if (result == 0)
            {
                Log.Warning("Failed to update banner in position {position}", model.Position);
                return ServiceResult.Fail($"Failed to update banner in position {model.Position}");
            }

            var newPosition = banner.Position;

            await cacheService.RemoveAsync($"Banner-{oldPosition}");
            await cacheService.RemoveAsync($"Banner-{newPosition}");

            Log.Information("Updated banner {bId} successfully", model.Id);
            return ServiceResult.Ok("Updated banner successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in BannerService.UpdateBannerAsync()");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> DeleteBannerAsync(Guid bannerId)
    {
        try
        {
            var banner = await bannerRepository.GetBannerByIdAsync(bannerId);
            if (banner == null)
            {
                Log.Warning("Failed to delete banner: Not found the banner");
                return ServiceResult.Fail("Not found the banner", HttpStatusCode.NotFound);
            }
            
            var cacheKey = $"Banner-{banner.Position}";
            
            var result = await bannerRepository.DeleteBannerAsync(banner);

            if (result == 0)
            {
                Log.Warning("Failed to delete banner {bId}", banner.Id);
                return ServiceResult.Fail("Failed to delete banner");
            }

            try
            {
                await fileService.DeleteFileAsync(banner.ImageUrl);
            }
            catch (Exception e)
            {
                Log.Warning(e, "Failed to delete image-file from banner");
            }

            await cacheService.RemoveAsync(cacheKey);
            
            Log.Information("Deleted banner successfully");
            return ServiceResult.Ok("Deleted banner successfully");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in BannerService.DeleteBannerAsync()");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<List<GetBannerDto>>> GetBannersAsync(string? position)
    {
        try
        {
            var cacheKey = $"Banner-{position}";
            
            var bannersInCache = await cacheService.GetAsync<List<GetBannerDto>>(cacheKey);

            if (bannersInCache == null)
            {
                var banners = await bannerRepository.GetBannersAsync(position);
                bannersInCache = mapper.Map<List<GetBannerDto>>(banners);

                await cacheService.AddAsync(cacheKey, bannersInCache, DateTimeOffset.Now.AddMinutes(10));
            }
            
            return ServiceResult<List<GetBannerDto>>.Ok(bannersInCache);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in BannerService.GetBannersAsync()");
            return ServiceResult<List<GetBannerDto>>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<GetBannerDto>> GetBannerByIdAsync(Guid bannerId)
    {
        try
        {
            var banner = await bannerRepository.GetBannerByIdAsync(bannerId);
            if (banner == null)
            {
                return ServiceResult<GetBannerDto>.Fail("Not found the banner", HttpStatusCode.NotFound);
            }

            var mappedBanner = mapper.Map<GetBannerDto>(banner);

            return ServiceResult<GetBannerDto>.Ok(mappedBanner);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in BannerService.GetBannerByIdAsync()");
            return ServiceResult<GetBannerDto>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }
}