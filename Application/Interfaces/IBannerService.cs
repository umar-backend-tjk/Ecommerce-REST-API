using Application.DTOs.BannerDtos;
using Domain.Responses;

namespace Application.Interfaces;

public interface IBannerService
{
    Task<ServiceResult> CreateBannerAsync(CreateBannerDto model);
    Task<ServiceResult> UpdateBannerAsync(UpdateBannerDto model);
    Task<ServiceResult> DeleteBannerAsync(Guid bannerId);
    Task<ServiceResult<List<GetBannerDto>>> GetBannersAsync(string? position);
    Task<ServiceResult<GetBannerDto>> GetBannerByIdAsync(Guid bannerId);
}