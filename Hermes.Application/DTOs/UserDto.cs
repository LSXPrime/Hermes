namespace Hermes.Application.DTOs;

public class UserDto 
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public AddressDto Address { get; set; }
    public string Role { get; set; }
}