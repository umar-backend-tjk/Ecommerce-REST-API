using System.Net;
using System.Security.Claims;
using Application.DTOs.Cart;
using Application.DTOs.CartItem;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Responses;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Infrastructure.Services;

public class CartService(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IMapper mapper,
    IHttpContextAccessor accessor) : ICartService
{
    public async Task<ServiceResult> DeleteCartAsync(Guid cartId)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var guidUserId = Guid.Parse(userId);
            
            var cart = await cartRepository.GetCartByIdAsync(cartId);
            if (cart == null)
                return ServiceResult.Fail("Cart not found", HttpStatusCode.NotFound);

            if (cart.UserId != guidUserId)
                return ServiceResult.Fail("Forbidden", HttpStatusCode.Forbidden);
            
            var result = await cartRepository.DeleteCartAsync(cart);

            return result > 0
                ? ServiceResult.Ok("Deleted cart successfully")
                : ServiceResult.Fail("Failed to delete a cart");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in CartService.DeleteCartAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult<GetCartDto>> GetCartAsync()
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var guidUserId = Guid.Parse(userId);

            var cart = await cartRepository.GetCartAsync(guidUserId);
            if (cart == null)
                return ServiceResult<GetCartDto>.Fail("Haven't created a cart yet");

            var mappedCarts = mapper.Map<GetCartDto>(cart);

            return new ServiceResult<GetCartDto>(mappedCarts);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in CartService.GetCartAsync");
            return ServiceResult<GetCartDto>.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> AddCartItemAsync(AddCartItemDto cartItemModel)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var guidUserId = Guid.Parse(userId);
            
            var cart = await cartRepository.GetCartByIdAsync(cartItemModel.CartId);
            if (cart == null)
                return ServiceResult.Fail("Not found the cart", HttpStatusCode.NotFound);
            
            if (cart.UserId != guidUserId)
                return ServiceResult.Fail("Forbidden", HttpStatusCode.Forbidden);

            var product = await productRepository.GetProductByIdAsync(cartItemModel.ProductId);
            if (product == null)
                return ServiceResult.Fail("Not found the product", HttpStatusCode.NotFound);

            var mappedCart = mapper.Map<CartItem>(cartItemModel);
            mappedCart.UnitPrice = product.Price;

            var result = await cartRepository.CreateCartItemAsync(mappedCart);

            return result > 0
                ? ServiceResult.Ok("Added product to cart successfully")
                : ServiceResult.Fail("Failed to add cartItem");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in CartService.AddCartItemAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }

    public async Task<ServiceResult> UpdateCartItemAsync(UpdateCartItemDto cartItemModel)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var guidUserId = Guid.Parse(userId);

            var cart = await cartRepository.GetCartAsync(guidUserId);
            if (cart == null)
                return ServiceResult.Fail("Not found the cart", HttpStatusCode.NotFound);

            var cartItem = cart.Items.FirstOrDefault(ci => ci.Id == cartItemModel.Id);
            if (cartItem == null)
                return ServiceResult.Fail("Not found the cartItem", HttpStatusCode.NotFound);

            cartItem.Quantity = cartItemModel.Quantity;

            var result = await cartRepository.UpdateCartItemAsync(cartItem);

            return result > 0
                ? ServiceResult.Ok("Updated cartItem successfully")
                : ServiceResult.Fail("Failed to update cartItem");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in CartService.UpdateCartItemAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }


    public async Task<ServiceResult> DeleteCartItemAsync(Guid cartItemId)
    {
        try
        {
            var userId = accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var guidUserId = Guid.Parse(userId);
            
            var cart = await cartRepository.GetCartAsync(guidUserId);
            if (cart == null)
                return ServiceResult.Fail("Not found the cart", HttpStatusCode.NotFound);
            
            var cartItem = cart.Items.FirstOrDefault(ci => ci.Id == cartItemId);
            if (cartItem == null)
                return ServiceResult.Fail("Not found the cartItem", HttpStatusCode.NotFound);
            
            var result = await cartRepository.DeleteCartItemAsync(cartItem);

            return result > 0
                ? ServiceResult.Ok("Deleted cartItem successfully")
                : ServiceResult.Fail("Failed to delete cartItem");
        }
        catch (Exception e)
        {
            Log.Error(e, "Unexpected error in CartService.DeleteCartItemAsync");
            return ServiceResult.Fail("Unexpected error", HttpStatusCode.InternalServerError);
        }
    }
}