using Application.DTOs.Auth;
using Application.DTOs.Category;
using Application.DTOs.Product;
using Application.DTOs.ProductDtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<RegisterDto, AppUser>();
        
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));;
        CreateMap<Category, GetCategoryDto>();

        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();
        CreateMap<Product, GetProductDto>();
    }
}