using Hermes.Application.DTOs;
using Hermes.Domain.Entities;

namespace Hermes.Application.Interfaces;

/// <summary>
/// Defines the interface for a service that handles shipping-related operations.
/// </summary>
public interface IShippingService
{
    /// <summary>
    /// Retrieves available shipping rates for a given set of shipping requests.
    /// </summary>
    /// <param name="request">An IEnumerable of ShippingRateRequest objects representing the shipping requests.</param>
    /// <returns>An IEnumerable of ShippingRate objects representing the available shipping rates.</returns>
    Task<IEnumerable<ShippingRate>> GetShippingRatesAsync(IEnumerable<ShippingRateRequest> request);

    /// <summary>
    /// Creates a new shipment.
    /// </summary>
    /// <param name="request">An IEnumerable of CreateShipmentRequest objects representing the shipment details.</param>
    /// <returns>A Shipment object representing the newly created shipment.</returns>
    Task<Shipment> CreateShipmentAsync(IEnumerable<CreateShipmentRequest> request);

    /// <summary>
    /// Tracks the status of a shipment based on its tracking number.
    /// </summary>
    /// <param name="trackingNumber">The tracking number of the shipment to track.</param>
    /// <returns>A TrackingInformation object containing the tracking details of the shipment.</returns>
    Task<TrackingInformation> TrackShipmentAsync(string trackingNumber);

    /// <summary>
    /// Cancels an existing shipment.
    /// </summary>
    /// <param name="trackingNumber">The tracking number of the shipment to cancel.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task CancelShipmentAsync(string trackingNumber);

    /// <summary>
    /// Calculates the shipping cost for a given cart.
    /// </summary>
    /// <param name="cart">The Cart object containing the items to ship.</param>
    /// <param name="userPostalCode">The user's postal code (optional).</param>
    /// <param name="carrier">The preferred shipping carrier (optional).</param>
    /// <param name="service">The preferred shipping service (optional).</param>
    /// <returns>The calculated shipping cost.</returns>
    Task<decimal> CalculateShipping(Cart cart, string userPostalCode = "", string carrier = "", string service = "");

    /// <summary>
    /// Calculates the tax amount for a given cart.
    /// </summary>
    /// <param name="cart">The Cart object containing the items to calculate tax for.</param>
    /// <returns>The calculated tax amount.</returns>
    Task<decimal> CalculateTax(Cart cart);
}