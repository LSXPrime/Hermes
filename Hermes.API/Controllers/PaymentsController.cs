using Hermes.API.Attributes;
using Hermes.Application.DTOs;
using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Enums;
using Hermes.Domain.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace Hermes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    IPaymentService paymentService,
    IOrderService orderService,
    IProductService productService,
    IOptions<StripeSettings> options) : ControllerBaseEx
{
    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost("create-checkout-session/{orderId:int}")]
    public async Task<IActionResult> CreateCheckoutSession(int orderId)
    {
        var order = await orderService.GetOrderByIdAsync(orderId);
        if (order == null || (order.UserId != CurrentUserId && CurrentUserRole != "Admin"))
        {
            return Forbid();
        }

        if (order.OrderStatus != OrderStatus.Pending)
        {
            return BadRequest("Order is not in a pending state.");
        }

        try
        {
            var checkoutSessionDto = new CreateCheckoutSessionDto
            {
                SuccessUrl = options.Value.SuccessUrl,
                CancelUrl = options.Value.CancelUrl,
                Currency = order.Currency,
                CartItems = (await Task.WhenAll(order.OrderItems.Select(async item =>
                {
                    var product = await productService.GetProductByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        throw new NotFoundException($"Product with ID {item.ProductId} not found.");
                    }

                    return new CartCheckoutItemDto
                    {
                        ProductName = product.Name,
                        Price = item.PriceAtPurchase,
                        Quantity = item.Quantity
                    };
                })).ConfigureAwait(false)).ToList()
            };

            var sessionId = await paymentService.CreateCheckoutSessionAsync(checkoutSessionDto);
            order.CheckoutSessionId = sessionId;
            await orderService.UpdateOrderAsync(orderId, order);

            return Ok(new { SessionId = sessionId });
        }
        catch (PaymentException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [AuthorizeMiddleware(["User", "Admin"])]
    [HttpPost("create-payment-intent/{orderId:int}")]
    public async Task<IActionResult> CreatePaymentIntent(int orderId)
    {
        var order = await orderService.GetOrderByIdAsync(orderId);
        if (order == null || (order.UserId != CurrentUserId && CurrentUserRole != "Admin"))
        {
            return Forbid();
        }

        if (order.OrderStatus != OrderStatus.Pending)
        {
            return BadRequest("Order is not in a pending state.");
        }

        try
        {
            var paymentIntentDto = new CreatePaymentIntentDto
            {
                Amount = order.TotalAmount,
                Currency = order.Currency,
                OrderId = order.Id
            };

            var clientSecret = await paymentService.CreatePaymentIntentAsync(paymentIntentDto);
            order.PaymentIntentId = clientSecret;
            await orderService.UpdateOrderAsync(orderId, order);

            return Ok(new { ClientSecret = clientSecret });
        }
        catch (PaymentException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }


    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            if (!await paymentService.ValidateWebhookSignatureAsync(json, Request.Headers["Stripe-Signature"]!, out var webHook))
            {
                return BadRequest("Invalid Stripe webhook signature.");
            }
            
            await paymentService.HandleWebhookEventAsync(webHook!);
            return Ok();
        }
        catch (StripeException)
        {
            return BadRequest("Invalid Stripe webhook signature.");
        }
    }
}