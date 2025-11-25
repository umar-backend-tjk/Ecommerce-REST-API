using Application.DTOs.Auth;
using Application.DTOs.Category;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<RegisterDto, AppUser>();
        
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<Category, GetCategoryDto>();
    }
}