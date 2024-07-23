using AutoMapper;
using Hermes.Application.DTOs;
using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Enums;
using Hermes.Domain.Interfaces;
using Hermes.Domain.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Hermes.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly string _stripeWebhookSecret;

    public StripePaymentService(IOptions<StripeSettings> options, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _stripeWebhookSecret = options.Value.WebhookSecret;
        StripeConfiguration.ApiKey = options.Value.SecretKey;
    }

    /// <summary>
    /// Creates a new payment intent.
    /// </summary>
    /// <param name="paymentIntentDto">The CreatePaymentIntentDto object containing the payment intent data to create.</param>
    /// <returns>A string representing the ID of the created payment intent.</returns>
    public async Task<string> CreatePaymentIntentAsync(CreatePaymentIntentDto paymentIntentDto)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(paymentIntentDto.Amount * 100),
            Currency = paymentIntentDto.Currency,
            PaymentMethodTypes = [paymentIntentDto.PaymentMethod],
            Metadata = new Dictionary<string, string>
            {
                { "orderId", paymentIntentDto.OrderId.ToString() }
            }
        };

        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            return paymentIntent.ClientSecret;
        }
        catch (StripeException? ex)
        {
            throw new PaymentException("An error occurred while processing the payment.", ex);
        }
        catch (Exception? ex)
        {
            throw new PaymentException("An unexpected error occurred during payment processing.", ex);
        }
    }

    /// <summary>
    /// Creates a new checkout session.
    /// </summary>
    /// <param name="checkoutSessionDto">The CreateCheckoutSessionDto object containing the checkout session data to create.</param>
    /// <returns>A string representing the ID of the created checkout session.</returns>
    public async Task<string> CreateCheckoutSessionAsync(CreateCheckoutSessionDto checkoutSessionDto)
    {
        var options = new SessionCreateOptions
        {
            SuccessUrl = checkoutSessionDto.SuccessUrl,
            CancelUrl = checkoutSessionDto.CancelUrl,
            PaymentMethodTypes = [checkoutSessionDto.PaymentMethod],
            LineItems = checkoutSessionDto.CartItems.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100),
                    Currency = checkoutSessionDto.Currency,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.ProductName,
                    }
                },
                Quantity = item.Quantity,
            }).ToList(),
            Mode = "payment",
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return session.Id;
    }

    /// <summary>
    /// Creates a new refund for a payment.
    /// </summary>
    /// <param name="refundDto">The CreateRefundDto object containing the refund data to create.</param>
    /// <returns>A RefundDto object representing the created refund, or null if the refund cannot be created.</returns>
    public async Task<RefundDto?> CreateRefundAsync(CreateRefundDto refundDto)
    {
        var options = new RefundCreateOptions
        {
            PaymentIntent = refundDto.PaymentIntentId,
            Amount = (long)(refundDto.Amount * 100),
            Reason = "requested_by_customer",
            Metadata = new Dictionary<string, string>
            {
                { "orderId", refundDto.OrderId.ToString() }
            }
        };

        try
        {
            var service = new RefundService();
            var refund = await service.CreateAsync(options);

            return _mapper.Map<RefundDto>(refund);
        }
        catch (StripeException ex)
        {
            throw new PaymentException("An error occurred while processing the refund.", ex);
        }
    }

    /// <summary>
    /// Validates the signature of a webhook payload.
    /// </summary>
    /// <param name="payload">The webhook payload to validate.</param>
    /// <param name="sigHeader">The signature header received from the webhook.</param>
    /// <param name="result">Optional output parameter to store any validation result message.</param>
    /// <returns>True if the signature is valid, false otherwise.</returns>
    public Task<bool> ValidateWebhookSignatureAsync(string payload, string sigHeader, out string? result)
    {
        try
        {
            result = EventUtility.ConstructEvent(payload, sigHeader, _stripeWebhookSecret).ToJson();
            return Task.FromResult(true);
        }
        catch (StripeException e)
        {
            throw new BadRequestException($"Invalid Stripe webhook signature. Error: {e.Message}");
        }
    }

    /// <summary>
    /// Handles a webhook event received from the payment provider.
    /// </summary>
    /// <param name="eventJson">The JSON payload of the webhook event.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task HandleWebhookEventAsync(string eventJson)
    {
        var stripeEvent = EventUtility.ParseEvent(eventJson);
        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (!int.TryParse(paymentIntent?.Metadata["orderId"], out var intentOrderId))
                    throw new PaymentException(
                        $"Invalid or missing orderId in PaymentIntent metadata: {paymentIntent?.Id}");

                await HandleOrderStatusUpdateAsync(intentOrderId, OrderStatus.Paid);
                break;
            case "checkout.session.completed":
                var session = stripeEvent.Data.Object as Session;
                if (!int.TryParse(session?.Metadata["orderId"], out var sessionOrderId))
                    throw new PaymentException($"Invalid or missing orderId in PaymentIntent metadata: {session?.Id}");

                await HandleOrderStatusUpdateAsync(sessionOrderId, OrderStatus.Paid);
                break;
            case "payment_intent.payment_failed":
                var paymentIntentFailed = stripeEvent.Data.Object as PaymentIntent;
                if (!int.TryParse(paymentIntentFailed?.Metadata["orderId"], out var paymentFailedOrderId))
                    throw new PaymentException(
                        $"Invalid or missing orderId in PaymentIntent metadata: {paymentIntentFailed?.Id}");

                await HandleOrderStatusUpdateAsync(paymentFailedOrderId, OrderStatus.Pending);
                break;
        }
    }
    
    /// <summary>
    /// Updates the status of an order based on the payment intent status.
    /// </summary>
    /// <param name="orderId">The ID of the order to update.</param>
    /// <param name="newStatus">The new status of the order.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task HandleOrderStatusUpdateAsync(int orderId, OrderStatus newStatus)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
        if (order == null)
            throw new PaymentException($"Order not found for Order Status Update on payment: {orderId}");

        order.OrderStatus = newStatus;
        await _unitOfWork.Orders.UpdateAsync(order);
    }
}