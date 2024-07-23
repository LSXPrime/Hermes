using Hermes.Application.DTOs;
using Hermes.Application.Exceptions;
using Hermes.Application.Interfaces;
using Hermes.Domain.Entities;
using Hermes.Domain.Interfaces;
using Shipment = Hermes.Application.DTOs.Shipment;

namespace Hermes.Infrastructure.Services;

// TODO: Implement Shippo API for multi shipping providers support, currently act as placeholder
public class MultiShippingService(IUnitOfWork unitOfWork) : IShippingService
{
    /*
    // ShippingRates API helps only in getting shipping rates, Shippo looks a better solution for the whole shipping process
    private readonly IConfiguration _configuration;
    private readonly RateManager _rateManager;

    public MultiShippingService(IConfiguration configuration)
    {
        _configuration = configuration;
        _rateManager = new RateManager();
        AddProviders();
    }

    private void AddProviders()
    {
        var providers = _configuration.GetSection("Providers").GetChildren().ToArray();
        var dhl = providers.FirstOrDefault(x => x.Key == "DHL");

        if (dhl != null)
            _rateManager.AddProvider(new DHLProvider(dhl["SiteId"], dhl["Password"], bool.Parse(dhl["IsProduction"]!)));
    }
    */

    /// <summary>
    /// Retrieves available shipping rates for a given set of shipping requests.
    /// </summary>
    /// <param name="request">An IEnumerable of ShippingRateRequest objects representing the shipping requests.</param>
    /// <returns>An IEnumerable of ShippingRate objects representing the available shipping rates.</returns>
    public async Task<IEnumerable<ShippingRate>> GetShippingRatesAsync(IEnumerable<ShippingRateRequest> request)
    {
        return new List<ShippingRate>
        {
            new()
            {
                Carrier = "DHL",
                ServiceName = "Ground",
                TotalRate = 10,
                BaseRate = 5,
                Tax = 2,
                Currency = "USD",
                EstimatedDeliveryDate = DateTime.UtcNow.AddDays(5)
            },
            new()
            {
                Carrier = "FedEx",
                ServiceName = "2nd Day Air",
                TotalRate = 10,
                BaseRate = 5,
                Tax = 2,
                Currency = "USD",
                EstimatedDeliveryDate = DateTime.UtcNow.AddDays(5)
            }
        };
    }

    /// <summary>
    /// Creates a new shipment.
    /// </summary>
    /// <param name="request">An IEnumerable of CreateShipmentRequest objects representing the shipment details.</param>
    /// <returns>A Shipment object representing the newly created shipment.</returns>
    public async Task<Shipment> CreateShipmentAsync(IEnumerable<CreateShipmentRequest> request)
    {
        return new Shipment
        {
            TrackingNumber = "123456789",
            ShippingLabelUrls = new List<string>
            {
                "https://example.com/label.png"
            }
        };
    }

    /// <summary>
    /// Tracks the status of a shipment based on its tracking number.
    /// </summary>
    /// <param name="trackingNumber">The tracking number of the shipment to track.</param>
    /// <returns>A TrackingInformation object containing the tracking details of the shipment.</returns>
    public async Task<TrackingInformation> TrackShipmentAsync(string trackingNumber)
    {
        TrackingInformation[] trackedItems =
        [
            new TrackingInformation
            {
                TrackingNumber = "123456789",
                Carrier = "FedEx",
                CurrentStatus = "Shipped",
                Events = new List<TrackingEvent>
                {
                    new()
                    {
                        Timestamp = DateTime.UtcNow,
                        Location = "New York, NY",
                        Description = "Shipped"
                    }
                }
            }
        ];
        
        var trackingInfo = trackedItems.FirstOrDefault(x => x.TrackingNumber == trackingNumber);
        if (trackingInfo == null)
            throw new NotFoundException("Tracking number not found");
        
        return trackingInfo;
    }

    /// <summary>
    /// Cancels an existing shipment.
    /// </summary>
    /// <param name="trackingNumber">The tracking number of the shipment to cancel.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task CancelShipmentAsync(string trackingNumber)
    {
    }

    /// <summary>
    /// Calculates the shipping cost for a given cart.
    /// </summary>
    /// <param name="cart">The Cart object containing the items to ship.</param>
    /// <param name="userPostalCode">The user's postal code (optional).</param>
    /// <param name="carrier">The preferred shipping carrier (optional).</param>
    /// <param name="service">The preferred shipping service (optional).</param>
    /// <returns>The calculated shipping cost.</returns>
    public async Task<decimal> CalculateShipping(Cart cart, string userPostalCode = "", string carrier = "",
        string service = "")
    {
        userPostalCode = string.IsNullOrEmpty(userPostalCode)
            ? (await unitOfWork.Users.GetByIdAsync(cart.UserId))?.Address.PostalCode!
            : userPostalCode;

        var itemsByOriginPostalCode = cart.CartItems.GroupBy(item => item.Product.Seller.Address.PostalCode).ToArray();
        var shippingRateRequests = itemsByOriginPostalCode.Select(group => new ShippingRateRequest
            {
                OriginPostalCode = group.Key,
                DestinationPostalCode = userPostalCode,
                Weight = group.Sum(item => item.Product.Weight),
                Width = group.Max(item => item.Product.Width),
                Height = group.Max(item => item.Product.Height),
                Length = group.Max(item => item.Product.Length)
            })
            .ToArray();

        var shippingRates = (await GetShippingRatesAsync(shippingRateRequests)).ToArray();
        return itemsByOriginPostalCode.Select(group => !string.IsNullOrEmpty(carrier) || !string.IsNullOrEmpty(service)
                ? shippingRates.FirstOrDefault(x =>
                    x.Carrier.Equals(carrier, StringComparison.OrdinalIgnoreCase) &&
                    x.ServiceName.Equals(service, StringComparison.OrdinalIgnoreCase))
                : shippingRates
                    .FirstOrDefault()) // Default to the first available shipping rate if no specific carrier/service match is found.
            .OfType<ShippingRate>()
            .Sum(shippingRate => shippingRate.TotalRate);
    }

    /// <summary>
    /// Calculates the tax amount for a given cart.
    /// </summary>
    /// <param name="cart">The Cart object containing the items to calculate tax for.</param>
    /// <returns>The calculated tax amount.</returns>
    public async Task<decimal> CalculateTax(Cart cart)
    {
        // TODO: Implement tax calculation logic
        return cart.TotalPrice + cart.ShippingCost * 0.1m;
    }
}