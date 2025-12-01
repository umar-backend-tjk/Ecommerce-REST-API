using Application.DTOs.OrderDtos;
using Domain.Entities;
using Domain.Enums;
using Domain.Responses;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<ServiceResult> CheckoutAsync(CreateOrderDto orderDto);
    Task<ServiceResult<List<GetOrderDto>>> GetMyOrdersAsync();
    Task<ServiceResult<List<GetOrderDto>>> GetAllOrdersAsync();
    Task<ServiceResult<GetOrderDto>> GetOrderByOrderNumberAsync(string orderNumber);
    Task<ServiceResult> CancelOrderAsync(string orderNumber);
    Task<ServiceResult> ChangeOrderStatusAsync(string orderNumber, OrderStatus status);
    Task<ServiceResult> ChangeOrderCurrencyAsync(string orderNumber, string currency, decimal rate);
}