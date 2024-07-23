using Hermes.API.Attributes;
using Hermes.Application.DTOs;
using Hermes.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.API.Controllers;

[Route("api/coupons")]
[ApiController]
public class CouponsController(ICouponService couponService) : ControllerBaseEx
{
    [AuthorizeMiddleware(["Admin"])]
    [HttpPost]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto couponDto)
    {
        var newCoupon = await couponService.CreateCouponAsync(couponDto);
        return newCoupon != null 
            ? CreatedAtAction(nameof(GetCouponById), new { id = newCoupon.Id }, newCoupon) 
            : BadRequest("Failed to create coupon");
    }

    [AuthorizeMiddleware(["Admin"])]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCoupon(int id, [FromBody] UpdateCouponDto couponDto)
    {
        var updatedCoupon = await couponService.UpdateCouponAsync(id, couponDto); 
        return updatedCoupon != null 
            ? Ok(updatedCoupon) 
            : BadRequest("Failed to update coupon.");
    }

    [AuthorizeMiddleware(["Admin"])]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCoupon(int id)
    {
        await couponService.DeleteCouponAsync(id); 
        return NoContent(); 
    }
    
    [AuthorizeMiddleware(["Admin"])]
    [HttpGet("active")] 
    public async Task<IActionResult> GetActiveCoupons()
    {
        var coupons = await couponService.GetActiveCouponsAsync(); 
        return Ok(coupons); 
    }

    [AuthorizeMiddleware(["Admin"])]
    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredCoupons()
    {
        var coupons = await couponService.GetExpiredCouponsAsync();
        return Ok(coupons);
    }

    [AuthorizeMiddleware(["Admin"])]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCouponById(int id)
    {
        var coupon = await couponService.GetCouponByIdAsync(id);
        return coupon != null ? Ok(coupon) : NotFound();
    }
}