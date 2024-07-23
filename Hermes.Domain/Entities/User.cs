namespace Hermes.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }

    public string Role { get; set; }

    // In case of a Seller role
    public int Rating { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiration { get; set; }

    // Navigation Properties
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
    public Cart Cart { get; set; }
    public int CartId { get; set; }

    public int AddressId { get; set; }
    public Address Address { get; set; }
}