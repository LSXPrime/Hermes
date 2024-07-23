using Hermes.Application.DTOs;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that handles payment-related operations.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Creates a new payment intent.
    /// </summary>
    /// <param name="paymentIntentDto">The CreatePaymentIntentDto object containing the payment intent data to create.</param>
    /// <returns>A string representing the ID of the created payment intent.</returns>
    Task<string> CreatePaymentIntentAsync(CreatePaymentIntentDto paymentIntentDto);

    /// <summary>
    /// Creates a new checkout session.
    /// </summary>
    /// <param name="checkoutSessionDto">The CreateCheckoutSessionDto object containing the checkout session data to create.</param>
    /// <returns>A string representing the ID of the created checkout session.</returns>
    Task<string> CreateCheckoutSessionAsync(CreateCheckoutSessionDto checkoutSessionDto);

    /// <summary>
    /// Creates a new refund for a payment.
    /// </summary>
    /// <param name="refundDto">The CreateRefundDto object containing the refund data to create.</param>
    /// <returns>A RefundDto object representing the created refund, or null if the refund cannot be created.</returns>
    Task<RefundDto?> CreateRefundAsync(CreateRefundDto refundDto);

    /// <summary>
    /// Validates the signature of a webhook payload.
    /// </summary>
    /// <param name="payload">The webhook payload to validate.</param>
    /// <param name="sigHeader">The signature header received from the webhook.</param>
    /// <param name="result">Optional output parameter to store any validation result message.</param>
    /// <returns>True if the signature is valid, false otherwise.</returns>
    Task<bool> ValidateWebhookSignatureAsync(string payload, string sigHeader, out string? result);

    /// <summary>
    /// Handles a webhook event received from the payment provider.
    /// </summary>
    /// <param name="eventJson">The JSON payload of the webhook event.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task HandleWebhookEventAsync(string eventJson);
}