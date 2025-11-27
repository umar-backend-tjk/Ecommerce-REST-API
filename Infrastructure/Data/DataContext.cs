using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Category>(entity =>
        {
            entity.HasKey(c => c.Id);
            
            entity.HasMany(c => c.ChildrenCategories)
                .WithOne(c => c.ParentCategory)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(c => c.Products)
                .WithOne(c => c.Category)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.HasMany(p => p.Images)
                .WithOne(img => img.Product)
                .HasForeignKey(img => img.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
         });
        
        base.OnModelCreating(builder);
    }
}