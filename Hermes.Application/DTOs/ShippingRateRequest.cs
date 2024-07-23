namespace Hermes.Application.DTOs;

public class ShippingRateRequest
{
    public string OriginPostalCode { get; set; }
    public string DestinationPostalCode { get; set; }
    public double Weight { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

public class ShippingRate 
{
    public string Carrier { get; set; }
    public string ServiceName { get; set; }
    public decimal TotalRate { get; set; }
    public decimal BaseRate { get; set; }
    public decimal Tax { get; set; }
    public string Currency { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
}

public class CreateShipmentRequest
{
    public string ShippingLabelUrl { get; set; }
    public string OriginPostalCode { get; set; }
    public string DestinationPostalCode { get; set; }
    public decimal PackageWeight { get; set; }
    public decimal PackageLength { get; set; }
    public decimal PackageWidth { get; set; }
    public decimal PackageHeight { get; set; }
    
}

public class Shipment
{
    public string TrackingNumber { get; set; }
    public IEnumerable<string> ShippingLabelUrls { get; set; }
}

public class TrackingInformation
{
    public string TrackingNumber { get; set; }
    public string Carrier { get; set; }
    public string CurrentStatus { get; set; }
    public IEnumerable<TrackingEvent> Events { get; set; } = new List<TrackingEvent>();
}

public class TrackingEvent
{
    public DateTime Timestamp { get; set; }
    public string Location { get; set; } 
    public string Description { get; set; }
}