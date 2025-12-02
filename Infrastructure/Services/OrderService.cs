using System.Net;
using System.Security.Claims;
using Application.DTOs.OrderDtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Responses;
using Hangfire;
using Infrastructure.Constants;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Infrastructure.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IMapper mapper,
    IHttpContextAccessor accessor) : IOrderService
{
    public async Task<ServiceResult> CheckoutAsync(CreateOrderDto orderDto)
    {
        var userIdString = accessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);

        Log.Information("Checkout started for user {UserId}", userId);

        var cart = await cartRepository.GetCartAsync(userId);
        if (cart == null || !cart.Items.Any())
        {
            Log.Warning("Cart not found or empty for user {UserId}", userId);
            return ServiceResult.Fail("Cart is empty");
        }

        Log.Information("Cart loaded for user {UserId}. ItemsCount={Count}", userId, cart.Items.Count);

        var orderNumber = await OrderNumberGenerator.GenerateOrderNumberAsync(orderRepository);

        Log.Information("Generated order number {OrderNumber} for user {UserId}", orderNumber, userId);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = orderDto.FullName,
            Phone = orderDto.Phone,
            Country = orderDto.Country,
            City = orderDto.City,
            AddressLine = orderDto.AddressLine,
            PostalCode = orderDto.PostalCode,
            Currency = orderDto.Currency,
            CurrencyRate = orderDto.CurrencyRate,
            OrderNumber = orderNumber,
            Status = OrderStatus.New,
            Items = new List<OrderItem>()
        };

        foreach (var ci in cart.Items)
        {
            order.Items.Add(new OrderItem
            {
                ProductId = ci.ProductId,
                ProductNameSnapshot = ci.Product!.Name,
                Quantity = ci.Quantity,
                UnitPriceBase = ci.UnitPrice,
                TotalBase = ci.UnitPrice * ci.Quantity
            });
        }

        order.TotalAmountBase = order.Items.Sum(i => i.TotalBase);

        Log.Information("Order entity created for user {UserId}. TotalBase={TotalAmount}",
            userId, order.TotalAmountBase);

        await using var transaction = await orderRepository.BeginTransactionAsync();

        try
        {
            await orderRepository.CreateOrderAsync(order);

            Log.Information("Order {OrderNumber} persisted. Clearing cart for user {UserId}",
                order.OrderNumber, userId);

            cart.Items.Clear();
            await cartRepository.SaveChangesAsync();

            await transaction.CommitAsync();

            Log.Information("Checkout completed for user {UserId}. Order={OrderNumber}, Cart cleared.",
                userId, order.OrderNumber);

            return ServiceResult.Ok("Created order successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            Log.Error(ex,
                "Transaction failed in CheckoutAsync for user {UserId}. OrderNumber={OrderNumber}",
                userId, orderNumber);

            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }


    public async Task<ServiceResult<List<GetOrderDto>>> GetMyOrdersAsync()
    {
        var userIdString = accessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userId = Guid.Parse(userIdString);

        try
        {
            var orders = await orderRepository.GetMyOrdersAsync(userId);

            var dto = mapper.Map<List<GetOrderDto>>(orders);

            return ServiceResult<List<GetOrderDto>>.Ok(dto);
        }
        catch (Exception)
        {
            return ServiceResult<List<GetOrderDto>>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }
    
    public async Task<ServiceResult<List<GetOrderDto>>> GetAllOrdersAsync()
    {
        try
        {
            Log.Information("Loading all orders");

            var orders = await orderRepository.GetAllOrdersAsync();
            Log.Information("Loaded {Count} orders", orders.Count);

            var mapped = mapper.Map<List<GetOrderDto>>(orders);

            return ServiceResult<List<GetOrderDto>>.Ok(mapped);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to load all orders");
            return ServiceResult<List<GetOrderDto>>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<GetOrderDto>> GetOrderByOrderNumberAsync(string orderNumber)
    {
        try
        {
            var user = accessor.HttpContext!.User;
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = user.IsInRole("Admin");
            var userId = Guid.Parse(userIdStr!);

            var order = await orderRepository.GetOrderByOrderNumberAsync(orderNumber);

            if (order == null)
                return ServiceResult<GetOrderDto>.Fail("Order not found", HttpStatusCode.NotFound);

            if (!isAdmin && order.UserId != userId)
                return ServiceResult<GetOrderDto>.Fail("Forbidden", HttpStatusCode.Forbidden);

            var dto = mapper.Map<GetOrderDto>(order);
            return ServiceResult<GetOrderDto>.Ok(dto);
        }
        catch (Exception)
        {
            return ServiceResult<GetOrderDto>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> CancelOrderAsync(string orderNumber)
    {
        Log.Information("Cancel request for order {OrderNumber}", orderNumber);

        try
        {
            var user = accessor.HttpContext!.User;
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = user.IsInRole("Admin");
            var userId = Guid.Parse(userIdStr!);

            var order = await orderRepository.GetOrderByOrderNumberAsync(orderNumber);

            if (order == null)
                return ServiceResult.Fail("Order not found", HttpStatusCode.NotFound);

            if (!isAdmin && order.UserId != userId)
                return ServiceResult.Fail("Forbidden", HttpStatusCode.Forbidden);

            if (order.Status is OrderStatus.Completed or OrderStatus.Cancelled)
                return ServiceResult.Fail("Order cannot be canceled");

            order.Status = OrderStatus.Cancelled;
            order.CanceledAt = DateTime.UtcNow;

            await orderRepository.UpdateOrderAsync(order);

            Log.Information("Order {OrderNumber} canceled successfully", orderNumber);

            return ServiceResult.Ok("Order canceled successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to cancel order {OrderNumber}", orderNumber);
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> ChangeOrderStatusAsync(string orderNumber, OrderStatus status)
    {
        Log.Information("Request to change status for order {OrderNumber} to {Status}", orderNumber, status);

        try
        {
            var user = accessor.HttpContext!.User;
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = user.FindFirstValue(ClaimTypes.Email)!;
            var isAdmin = user.IsInRole("Admin");
            var userId = Guid.Parse(userIdStr!);

            var order = await orderRepository.GetOrderByOrderNumberAsync(orderNumber);

            if (order == null)
                return ServiceResult.Fail("Order not found", HttpStatusCode.NotFound);

            if (!isAdmin && order.UserId != userId)
                return ServiceResult.Fail("Forbidden", HttpStatusCode.Forbidden);

            if (order.Status == OrderStatus.Cancelled)
                return ServiceResult.Fail("Cannot change status of a canceled order");

            order.Status = status;

            switch (status)
            {
                case OrderStatus.Completed:
                    order.CompletedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Paid:
                    order.PaidAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Shipped:
                    order.ShippedAt = DateTime.UtcNow;
                    break;
                case OrderStatus.Cancelled:
                    order.CanceledAt = DateTime.UtcNow;
                    break;
            }

            await orderRepository.UpdateOrderAsync(order);

            BackgroundJob.Enqueue<IEmailService>(x =>
                x.SendEmailAsync(userEmail, $"Changed status of order {order.OrderNumber} to {status}", HtmlPages.WelcomeMail));
            
            Log.Information("Order {OrderNumber} status updated to {Status}", orderNumber, status);

            return ServiceResult.Ok("Order status updated successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to change status for order {OrderNumber}", orderNumber);
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> ChangeOrderCurrencyAsync(string orderNumber, string currency, decimal rate)
    {
        Log.Information("Changing currency of order {OrderNumber} to {Currency} with rate {Rate}",
            orderNumber, currency, rate);

        try
        {
            var order = await orderRepository.GetOrderByOrderNumberAsync(orderNumber);

            if (order == null)
            {
                Log.Warning("Order {OrderNumber} not found for currency change", orderNumber);
                return ServiceResult.Fail("Order not found", HttpStatusCode.NotFound);
            }

            if (rate <= 0)
            {
                Log.Warning("Invalid currency rate {Rate} for {Currency}", rate, currency);
                return ServiceResult.Fail("Invalid currency rate");
            }

            order.Currency = currency;
            order.CurrencyRate = rate;

            await orderRepository.UpdateOrderAsync(order);

            Log.Information("Order {OrderNumber} currency updated to {Currency} (Rate={Rate})",
                orderNumber, currency, rate);

            return ServiceResult.Ok("Currency updated successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error changing currency for order {OrderNumber}", orderNumber);
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }
}