using Hermes.Application.DTOs; 

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that handles email sending operations.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an order confirmation email to the customer.
    /// </summary>
    /// <param name="order">The OrderDto representing the order for which to send the confirmation email.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task SendOrderConfirmationEmailAsync(OrderDto order);

    /// <summary>
    /// Sends an email to the customer with an update on the shipping status of their order.
    /// </summary>
    /// <param name="order">The OrderDto representing the order for which to send the shipping update email.</param>
    /// <param name="shippingStatus">The current shipping status of the order.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task SendShippingUpdateEmailAsync(OrderDto order, string shippingStatus); 

    /// <summary>
    /// Sends a password reset email to the user with a reset token.
    /// </summary>
    /// <param name="userEmail">The email address of the user to send the password reset email to.</param>
    /// <param name="resetToken">The token to be used for password reset.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task SendPasswordResetEmailAsync(string userEmail, string resetToken);
}