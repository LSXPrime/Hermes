using AutoMapper;
using Hermes.Application.DTOs;
using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;

namespace Hermes.Application.Services;

public class CartService(IUnitOfWork unitOfWork, IInventoryService inventoryService, ICouponService couponService, IShippingService shippingService, IMapper mapper) : ICartService
{
    /// <summary>
    /// Retrieves the cart associated with the specified user ID.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to retrieve.</param>
    /// <returns>A CartDto object representing the user's cart, or null if no cart is found.</returns>
    public async Task<CartDto?> GetCartByUserIdAsync(int userId)
    {
        var cart = await unitOfWork.Carts.GetCartByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId, TotalPrice = 0 };
            await unitOfWork.Carts.AddAsync(cart); 
        }
        
        return mapper.Map<CartDto>(cart); 
    }

    /// <summary>
    /// Adds an item to the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to modify.</param>
    /// <param name="productVariantId">The ID of the product variant to add to the cart.</param>
    /// <param name="quantity">The quantity of the product variant to add.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task AddItemToCartAsync(int userId, int productVariantId, int quantity)
    {
        Cart? cart = await unitOfWork.Carts.GetCartByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            await unitOfWork.Carts.AddAsync(cart);
        }
        
        var productVariant = await unitOfWork.Products.GetProductVariantByIdAsync(productVariantId);
        if (productVariant == null)
            throw new NotFoundException($"Product variant with ID {productVariantId} not found."); 

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productVariant.ProductId);
        var product = await unitOfWork.Products.GetByIdAsync(productVariant.ProductId);
        if (product == null) 
            throw new NotFoundException($"Product with ID {productVariant.ProductId} not found."); 
        if (!await inventoryService.IsInStockAsync(productVariant.ProductId, quantity))
            throw new OutOfStockException($"Product with ID {productVariant.ProductId} is out of stock or insufficient quantity available.");
        
        if (cartItem != null)
        {
            cartItem.Quantity += quantity;
            cartItem.PriceAtPurchase = product.Price + productVariant.PriceAdjustment;
            await unitOfWork.CartItems.UpdateAsync(cartItem);
        }
        else
        {
            cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductName = product.Name,
                ProductId = productVariant.ProductId,
                Quantity = quantity,
                PriceAtPurchase = product.Price + productVariant.PriceAdjustment
            };
            await unitOfWork.CartItems.AddAsync(cartItem);
        }
        
        cart.TotalPrice = CalculateTotalAmount(cart.CartItems);
        cart.ShippingCost = await shippingService.CalculateShipping(cart);
        cart.TaxAmount = await shippingService.CalculateTax(cart);
        
        await unitOfWork.Carts.UpdateAsync(cart); 
    }

    /// <summary>
    /// Updates the quantity of an item in the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to modify.</param>
    /// <param name="productId">The ID of the product whose quantity to update.</param>
    /// <param name="newQuantity">The new quantity for the product.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task UpdateCartItemQuantityAsync(int userId, int productId, int newQuantity)
    {
        var cart = await unitOfWork.Carts.GetCartByUserIdAsync(userId);
        var cartItem = cart?.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
        if (!await inventoryService.IsInStockAsync(productId, newQuantity))
            throw new OutOfStockException($"Product with ID {productId} is out of stock or insufficient quantity available.");
        
        if (cart != null && cartItem != null)
        {
            if (newQuantity <= 0)
            {
                await unitOfWork.CartItems.DeleteAsync(cartItem);
            }
            else
            {
                cartItem.Quantity = newQuantity;
                await unitOfWork.CartItems.UpdateAsync(cartItem);
            }

            cart.TotalPrice = CalculateTotalAmount(cart.CartItems);
            await unitOfWork.Carts.UpdateAsync(cart);
        }
    }

    /// <summary>
    /// Removes an item from the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to modify.</param>
    /// <param name="productId">The ID of the product to remove from the cart.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task RemoveItemFromCartAsync(int userId, int productId)
    {
        var cart = await unitOfWork.Carts.GetCartByUserIdAsync(userId);
        var cartItem = cart?.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

        if (cart != null && cartItem != null)
        {
            await unitOfWork.CartItems.DeleteAsync(cartItem);
            
            cart.TotalPrice = CalculateTotalAmount(cart.CartItems);
            await unitOfWork.Carts.UpdateAsync(cart);
        }
    }

    /// <summary>
    /// Clears all items from the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to clear.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task ClearCartAsync(int userId)
    {
        var cart = await unitOfWork.Carts.GetCartByUserIdAsync(userId);
        if (cart != null)
        {
            await unitOfWork.Carts.ClearCartAsync(cart.Id);
        }
    }

    /// <summary>
    /// Applies a coupon to the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to modify.</param>
    /// <param name="couponCode">The code of the coupon to apply.</param>
    /// <returns>A CartDto object representing the user's cart after applying the coupon, or null if the coupon is invalid or cannot be applied.</returns>
    public async Task<CartDto?> ApplyCouponAsync(int userId, string couponCode) 
    {
        var cart = await unitOfWork.Carts.GetCartByUserIdAsync(userId);
        if (cart == null)
        {
            throw new NotFoundException($"Cart not found for user with ID {userId}.");
        }

        return await couponService.ApplyCouponAsync(cart.Id, couponCode);
    }

    /// <summary>
    /// Removes any applied coupon from the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user whose cart to modify.</param>
    /// <returns>A CartDto object representing the user's cart after removing the coupon.</returns>
    public async Task<CartDto?> RemoveCouponAsync(int userId)
    {
        var cart = await unitOfWork.Carts.GetCartByUserIdAsync(userId);
        if (cart == null)
        {
            throw new NotFoundException($"Cart not found for user with ID {userId}.");
        }

        return await couponService.RemoveCouponAsync(cart.Id);
    }

    /// <summary>
    /// Calculates the total amount of the user's cart.
    /// </summary>
    /// <param name="cartItems">The items in the user's cart.</param>
    /// <returns>The total amount of the user's cart.</returns>
    private decimal CalculateTotalAmount(IEnumerable<CartItem> cartItems)
    {
        decimal total = 0;
        foreach (var item in cartItems)
        {
            total += item.Quantity * item.PriceAtPurchase;
        }

        return total;
    }
}