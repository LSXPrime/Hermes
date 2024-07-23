using Hermes.API.Attributes;
using Hermes.Application.DTOs;
using Hermes.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Hermes.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShippingController(IShippingService shippingService) : ControllerBaseEx
{
    [HttpPost("rates")]
    public async Task<IActionResult> GetShippingRates([FromBody] List<ShippingRateRequest> requests)
    {
        var rates = await shippingService.GetShippingRatesAsync(requests);
        return Ok(rates);
    }

    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPost("create-shipment")]
    public async Task<IActionResult> CreateShipment([FromBody] List<CreateShipmentRequest> requests)
    {
        var shipment = await shippingService.CreateShipmentAsync(requests);
        return Ok(shipment);
    }

    [AuthorizeMiddleware(["User", "Admin", "Seller"])]
    [HttpGet("track/{trackingNumber}")]
    public async Task<IActionResult> TrackShipment(string trackingNumber)
    {
        var trackingInfo = await shippingService.TrackShipmentAsync(trackingNumber);
        return Ok(trackingInfo);
    }

    [AuthorizeMiddleware(["Admin", "Seller"])]
    [HttpPost("cancel/{trackingNumber}")]
    public async Task<IActionResult> CancelShipment(string trackingNumber)
    {
        await shippingService.CancelShipmentAsync(trackingNumber);
        return NoContent();
    }
}