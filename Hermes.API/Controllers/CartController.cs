using Hermes.API.Attributes;
using Hermes.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController(ICartService cartService) : ControllerBaseEx
{
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await cartService.GetCartByUserIdAsync(CurrentUserId);
        return cart != null ? Ok(cart) : NotFound();
    }

    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost("items/{productVariantId:int}/{quantity:int}")]
    public async Task<IActionResult> AddItemToCart(int productVariantId, int quantity)
    {
        await cartService.AddItemToCartAsync(CurrentUserId, productVariantId, quantity);
        return NoContent();
    }

    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPut("items/{productId:int}/{quantity:int}")]
    public async Task<IActionResult> UpdateCartItemQuantity(int productId, int quantity)
    {
        await cartService.UpdateCartItemQuantityAsync(CurrentUserId, productId, quantity);
        return NoContent();
    }

    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItemFromCart(int productId)
    {
        await cartService.RemoveItemFromCartAsync(CurrentUserId, productId);
        return NoContent();
    }

    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        await cartService.ClearCartAsync(CurrentUserId);
        return NoContent();
    }

    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost("coupons/{couponCode}")]
    public async Task<IActionResult> ApplyCoupon(string couponCode)
    {
        var cart = await cartService.ApplyCouponAsync(CurrentUserId, couponCode);
        return cart != null ? Ok(cart) : NotFound();
    }

    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpDelete("coupons")]
    public async Task<IActionResult> RemoveCoupon()
    {
        var cart = await cartService.RemoveCouponAsync(CurrentUserId);
        return cart != null ? Ok(cart) : NotFound();
    }
}