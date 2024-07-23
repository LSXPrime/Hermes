using Hermes.Domain.Entities;

namespace Hermes.Application.DTOs;

public class RegisterDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public AddressDto Address { get; set; }
    public string Role { get; set; }
}