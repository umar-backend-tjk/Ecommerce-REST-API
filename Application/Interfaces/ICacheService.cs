namespace Application.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task AddAsync<T>(string key, T value, DateTimeOffset expirationTime, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}