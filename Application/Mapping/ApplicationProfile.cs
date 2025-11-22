using Application.DTOs.Auth;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapping;

public class ApplicationProfile : Profile
{
    public ApplicationProfile()
    {
        CreateMap<RegisterDto, AppUser>();
    }
}