using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class FileStorageService(string rootPath) : IFileStorageService
{
    public async Task<string> SaveFileAsync(IFormFile file, string relativePath)
    {
        var folder = Path.Combine(rootPath, "wwwroot", relativePath);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(folder, fileName);
        
        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);
        
        return Path.Combine(relativePath, fileName).Replace('\\', '/');
    }

    public Task DeleteFileAsync(string relativePath)
    {
        var full = Path.Combine(rootPath, "wwwroot", relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
        if (File.Exists(full)) File.Delete(full);
        return Task.CompletedTask;
    }
}