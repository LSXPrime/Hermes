using AutoMapper;
using Hermes.Application.DTOs;
using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Entities;
using Hermes.Domain.Enums;
using Hermes.Domain.Interfaces;

namespace Hermes.Application.Services;

public class CouponService(IUnitOfWork unitOfWork, IMapper mapper) : ICouponService
{
    /// <summary>
    /// Retrieves a specific coupon by its ID.
    /// </summary>
    /// <param name="id">The ID of the coupon to retrieve.</param>
    /// <returns>A CouponDto object representing the specified coupon, or null if no coupon with the given ID is found.</returns>
    public async Task<CouponDto?> GetCouponByIdAsync(int id)
    {
        var coupon = await unitOfWork.Coupons.GetByIdAsync(id);
        if (coupon == null)
        {
            throw new NotFoundException("Coupon not found.");
        }
        
        return mapper.Map<CouponDto>(coupon);
    }

    /// <summary>
    /// Applies a coupon to a cart.
    /// </summary>
    /// <param name="cartId">The ID of the cart to apply the coupon to.</param>
    /// <param name="couponCode">The code of the coupon to apply.</param>
    /// <returns>A CartDto object representing the cart after applying the coupon, or null if the coupon is invalid or cannot be applied.</returns>
    public async Task<CartDto?> ApplyCouponAsync(int cartId, string couponCode)
    {
        var cart = await unitOfWork.Carts.GetByIdAsync(cartId);
        if (cart == null)
        {
            throw new NotFoundException($"Cart with ID {cartId} not found.");
        }

        var coupon = await unitOfWork.Coupons.GetByCodeAsync(couponCode);
        if (coupon == null || !await IsCouponValidAsync(coupon, cart.TotalPrice))
        {
            throw new BadRequestException("Invalid or inactive coupon code.");
        }

        if (cart.AppliedCouponCode == coupon.Code)
        {
            throw new BadRequestException("Coupon has already been applied to the cart.");
        }

        cart.TotalPrice -= CalculateDiscount(coupon, cart.TotalPrice); 

        cart.AppliedCouponCode = coupon.Code;
        cart.AppliedCoupon = coupon;

        await unitOfWork.Carts.UpdateAsync(cart);

        return mapper.Map<CartDto>(cart);
    }

    /// <summary>
    /// Removes a coupon from a cart.
    /// </summary>
    /// <param name="cartId">The ID of the cart to remove the coupon from.</param>
    /// <returns>A CartDto object representing the cart after removing the coupon.</returns>
    public async Task<CartDto?> RemoveCouponAsync(int cartId)
    {
        var cart = await unitOfWork.Carts.GetByIdAsync(cartId);
        if (cart == null)
        {
            throw new NotFoundException($"Cart with ID {cartId} not found.");
        }
        
        if (string.IsNullOrEmpty(cart.AppliedCouponCode))
        {
            return mapper.Map<CartDto>(cart);
        }

        var coupon = await unitOfWork.Coupons.GetByCodeAsync(cart.AppliedCouponCode);
        if (coupon != null)
        {
            cart.TotalPrice += CalculateDiscount(coupon, cart.TotalPrice); 
        }

        cart.AppliedCouponCode = null;
        cart.AppliedCoupon = null;
        await unitOfWork.Carts.UpdateAsync(cart);
        return mapper.Map<CartDto>(cart);
    }

    /// <summary>
    /// Checks if a coupon is valid and can be applied to a cart.
    /// </summary>
    /// <param name="couponCode">The code of the coupon to check.</param>
    /// <param name="cartTotal">The total amount of the cart to check the coupon against.</param>
    /// <returns>True if the coupon is valid and can be applied, false otherwise.</returns>
    public async Task<bool> IsCouponValidAsync(string couponCode, decimal cartTotal)
    {
        var coupon = await unitOfWork.Coupons.GetByCodeAsync(couponCode);
        return coupon != null && await IsCouponValidAsync(coupon, cartTotal);
    }

    /// <summary>
    /// Checks if a coupon is valid and can be applied to a cart.
    /// </summary>
    /// <param name="coupon">The coupon to check.</param>
    /// <param name="cartTotal">The total amount of the cart to check the coupon against.</param>
    /// <returns>True if the coupon is valid and can be applied, false otherwise.</returns>
    private Task<bool> IsCouponValidAsync(Coupon coupon, decimal cartTotal)
    {
        return Task.FromResult(coupon is { IsActive: true } && !(coupon.StartDate != null && coupon.StartDate > DateTime.UtcNow ||
                                                                 (coupon.EndDate != null && coupon.EndDate < DateTime.UtcNow)) &&
                               (!(coupon.MinimumOrderAmount > 0) || !(cartTotal < coupon.MinimumOrderAmount)));
    }

    /// <summary>
    /// Creates a new coupon.
    /// </summary>
    /// <param name="couponDto">The CreateCouponDto object containing the coupon data to create.</param>
    /// <returns>A CouponDto object representing the newly created coupon.</returns>
    public async Task<CouponDto?> CreateCouponAsync(CreateCouponDto couponDto)
    {
        var existingCoupon = await unitOfWork.Coupons.GetByCodeAsync(couponDto.Code);
        if (existingCoupon != null)
        {
            throw new BadRequestException($"Coupon with code {couponDto.Code} already exists.");
        }
        
        if (couponDto is { StartDate: not null, EndDate: not null } && couponDto.StartDate > couponDto.EndDate)
        {
            throw new BadRequestException("Start date must be before end date.");
        }

        var coupon = mapper.Map<Coupon>(couponDto);
        await unitOfWork.Coupons.AddAsync(coupon);
        return mapper.Map<CouponDto>(coupon);
    }

    /// <summary>
    /// Updates an existing coupon.
    /// </summary>
    /// <param name="couponId">The ID of the coupon to update.</param>
    /// <param name="couponDto">The UpdateCouponDto object containing the updated coupon data.</param>
    /// <returns>A CouponDto object representing the updated coupon.</returns>
    public async Task<CouponDto?> UpdateCouponAsync(int couponId, UpdateCouponDto couponDto)
    {
        var coupon = await unitOfWork.Coupons.GetByIdAsync(couponId);
        if (coupon == null)
        {
            throw new NotFoundException($"Coupon with ID {couponId} not found.");
        }
        
        if (couponDto is { StartDate: not null, EndDate: not null } && couponDto.StartDate > couponDto.EndDate)
        {
            throw new BadRequestException("Start date must be before end date.");
        }

        mapper.Map(couponDto, coupon); 
        await unitOfWork.Coupons.UpdateAsync(coupon);
        return mapper.Map<CouponDto>(coupon);
    }

    /// <summary>
    /// Deletes an existing coupon.
    /// </summary>
    /// <param name="couponId">The ID of the coupon to delete.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task DeleteCouponAsync(int couponId)
    {
        var coupon = await unitOfWork.Coupons.GetByIdAsync(couponId);
        if (coupon == null)
        {
            throw new NotFoundException($"Coupon with ID {couponId} not found.");
        }

        await unitOfWork.Coupons.DeleteAsync(coupon);
    }

    /// <summary>
    /// Retrieves a collection of active coupons.
    /// </summary>
    /// <returns>An IEnumerable of CouponDto objects representing active coupons.</returns>
    public async Task<IEnumerable<CouponDto>> GetActiveCouponsAsync()
    {
        var coupons = await unitOfWork.Coupons.GetActiveCouponsAsync();
        return mapper.Map<IEnumerable<CouponDto>>(coupons);
    }

    /// <summary>
    /// Retrieves a collection of expired coupons.
    /// </summary>
    /// <returns>An IEnumerable of CouponDto objects representing expired coupons.</returns>
    public async Task<IEnumerable<CouponDto>> GetExpiredCouponsAsync()
    {
        var coupons = await unitOfWork.Coupons.GetExpiredCouponsAsync();
        return mapper.Map<IEnumerable<CouponDto>>(coupons);
    }
    
    /// <summary>
    /// Calculates the discount amount based on the coupon type.
    /// </summary>
    /// <param name="coupon">The coupon to calculate the discount for.</param>
    /// <param name="cartTotal">The total amount of the cart.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Thrown if the coupon type is invalid.</exception>
    private decimal CalculateDiscount(Coupon coupon, decimal cartTotal)
    {
        return coupon.CouponType switch
        {
            CouponType.Amount => coupon.DiscountAmount,
            CouponType.Percentage => cartTotal * (coupon.DiscountAmount / 100),
            _ => throw new ArgumentException("Invalid coupon type.")
        };
    }
}