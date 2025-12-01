using Application.DTOs.OrderDtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories;

public class OrderRepository(DataContext context) : IOrderRepository
{
    public async Task<int> CreateOrderAsync(Order order)
    {
        await context.Orders.AddAsync(order);
        return await context.SaveChangesAsync();
    }

    public async Task<int> UpdateOrderAsync(Order order)
    {
        context.Orders.Update(order);
        return await context.SaveChangesAsync();
    }

    public async Task<int> DeleteOrderAsync(Order order)
    {
        context.Orders.Remove(order);
        return await context.SaveChangesAsync();
    }

    public async Task<List<Order>> GetMyOrdersAsync(Guid userId)
    {
        return await context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId && o.Status != OrderStatus.Cancelled)
            .ToListAsync();
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await context.Orders
            .Include(o => o.Items)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderByOrderNumberAsync(string orderNumber)
    {
        return await context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }
    
    public async Task<string?> GetLastOrderNumberAsync(int year, int month)
    {
        return await context.Orders
            .Where(o => o.CreatedAt.Year == year && o.CreatedAt.Month == month)
            .OrderByDescending(o => o.CreatedAt)
            .ThenByDescending(o => o.OrderNumber)
            .Select(o => o.OrderNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
        => await context.Database.BeginTransactionAsync();
}