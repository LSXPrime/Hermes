namespace Hermes.Domain.Entities;

public class Address : BaseEntity
{ 
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; } 
}