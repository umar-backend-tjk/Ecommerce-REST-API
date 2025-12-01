using Application.DTOs.OrderDtos;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Interfaces;

public interface IOrderRepository
{
    Task<int> CreateOrderAsync(Order order);
    Task<int> UpdateOrderAsync(Order order);
    Task<int> DeleteOrderAsync(Order order);
    Task<List<Order>> GetMyOrdersAsync(Guid userId);
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByOrderNumberAsync(string orderNumber);
    Task<string?> GetLastOrderNumberAsync(int year, int month);
    Task<IDbContextTransaction> BeginTransactionAsync();
}