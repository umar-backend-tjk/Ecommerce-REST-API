using Application.DTOs.OrderDtos;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    [Authorize]
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout(CreateOrderDto orderDto)
    {
        var result = await orderService.CheckoutAsync(orderDto);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var result = await orderService.GetMyOrdersAsync();
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpGet("{orderNumber}")]
    public async Task<IActionResult> GetOrderByOrderNumber(string orderNumber)
    {
        var result = await orderService.GetOrderByOrderNumberAsync(orderNumber);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize]
    [HttpPost("{orderNumber}/cancel")]
    public async Task<IActionResult> CancelOrder(string orderNumber)
    {
        var result = await orderService.ChangeOrderStatusAsync(orderNumber, OrderStatus.Cancelled);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        var result = await orderService.GetAllOrdersAsync();
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{orderNumber}/status")]
    public async Task<IActionResult> ChangeOrderStatus(string orderNumber, OrderStatus orderStatus)
    {
        var result = await orderService.ChangeOrderStatusAsync(orderNumber, orderStatus);
        return StatusCode(result.StatusCode, result);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{orderNumber}/currency")]
    public async Task<IActionResult> ChangeOrderCurrency(string orderNumber, string currency, decimal rate)
    {
        var result = await orderService.ChangeOrderCurrencyAsync(orderNumber, currency, rate);
        return StatusCode(result.StatusCode, result);
    }
    
    
}