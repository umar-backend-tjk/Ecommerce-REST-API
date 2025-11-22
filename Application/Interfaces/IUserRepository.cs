using Domain.Responses;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task<ServiceResult> UpdateUserAsync();
}