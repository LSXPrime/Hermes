using Hermes.Application.DTOs;
using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Interfaces;
using Hermes.Domain.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Hermes.Infrastructure.Services;

public class EmailService(IOptions<EmailSettings> options, IUnitOfWork unitOfWork) : IEmailService
{
    /// <summary>
    /// Sends an order confirmation email to the customer.
    /// </summary>
    /// <param name="order">The OrderDto representing the order for which to send the confirmation email.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SendOrderConfirmationEmailAsync(OrderDto order)
    {
        var user = await unitOfWork.Users.GetByIdAsync(order.UserId);
        if (user == null)
            throw new NotFoundException("User not found.");
        
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(options.Value.SenderName, options.Value.SenderEmail)); 
        email.To.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", user.Email)); 
        email.Subject = "Hermes E-Commerce - Order Confirmation"; 

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"<h1>Thank You for Your Order!</h1>
                      <p>Dear {user.FirstName} {user.LastName},</p>
                      <p>Thank you for your recent order with Hermes E-Commerce. We're excited to get your order processed and on its way to you.</p>
                      <p><strong>Order Details:</strong></p>
                      <ul>
                          <li>Order ID: {order.Id}</li>
                          <li>Order Date: {order.OrderDate}</li>
                          <li>Total Amount: {order.TotalAmount}</li>
                      </ul>
                      <p>We will send you another email when your order has been shipped.</p>
                      <p>Thank you for shopping with us!</p>" 
        };

        email.Body = bodyBuilder.ToMessageBody(); 

        await SendEmailAsync(email);
    }

    /// <summary>
    /// Sends an email to the customer with an update on the shipping status of their order.
    /// </summary>
    /// <param name="order">The OrderDto representing the order for which to send the shipping update email.</param>
    /// <param name="shippingStatus">The current shipping status of the order.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SendShippingUpdateEmailAsync(OrderDto order, string shippingStatus)
    {
        var user = await unitOfWork.Users.GetByIdAsync(order.UserId);
        if (user == null)
            throw new NotFoundException("User not found.");
        
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(options.Value.SenderName, options.Value.SenderEmail)); 
        email.To.Add(new MailboxAddress($"{user.FirstName} {user.LastName}", user.Email)); 
        email.Subject = "Hermes E-Commerce - Shipping Update";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"<h1>Shipping Update</h1>
                      <p>Dear {user.FirstName} {user.LastName},</p>
                      <p>Thank you for your recent order with Hermes E-Commerce. We're excited to get your order processed and on its way to you.</p>
                      <p><strong>Order Details:</strong></p>
                      <ul>
                          <li>Order ID: {order.Id}</li>
                          <li>Order Date: {order.OrderDate}</li>
                          <li>Total Amount: {order.TotalAmount}</li>
                          <li>Shipping Status: {shippingStatus}</li>
                      </ul>
                      <p>Thank you for shopping with us!</p>" 
        };

        email.Body = bodyBuilder.ToMessageBody();
        
        await SendEmailAsync(email);
    }

    /// <summary>
    /// Sends a password reset email to the user with a reset token.
    /// </summary>
    /// <param name="userEmail">The email address of the user to send the password reset email to.</param>
    /// <param name="resetToken">The token to be used for password reset.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SendPasswordResetEmailAsync(string userEmail, string resetToken)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(options.Value.SenderName, options.Value.SenderEmail)); 
        email.To.Add(new MailboxAddress($"{userEmail}", userEmail)); 
        email.Subject = "Hermes E-Commerce - Password Reset";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"<h1>Password Reset</h1>
                      <p>Dear '{userEmail}' account's owner,</p>
                      <p>You requested a password reset. Please click the link below to reset your password:</p>
                      <p><a href='{options.Value.PasswordResetEndpoint}{resetToken}'>Reset Password</a></p>
                      <p>This link will expire in 1 hour.</p>
                      <p>If you did not request a password reset, please ignore this email.</p>""
                      <p>Thank you for using Hermes E-Commerce!</p>" 
        };

        email.Body = bodyBuilder.ToMessageBody();
        
        await SendEmailAsync(email);
    }
    
    /// <summary>
    /// Sends an email using the provided MimeMessage object.
    /// </summary>
    /// <param name="email">The MimeMessage object representing the email to be sent.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task SendEmailAsync(MimeMessage email)
    {
        using var smtpClient = new SmtpClient();
        await smtpClient.ConnectAsync(options.Value.SmtpHost, options.Value.SmtpPort, SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(options.Value.SmtpUsername, options.Value.SmtpPassword);
        await smtpClient.SendAsync(email);
        await smtpClient.DisconnectAsync(true); 
    }
}