using FluentValidation;
using Hermes.Application.DTOs;

namespace Hermes.API.Validators;

public class ShipmentValidator : AbstractValidator<Shipment>
{
    public ShipmentValidator()
    {
        RuleFor(x => x.TrackingNumber)
            .NotEmpty().WithMessage("Tracking number is required.");

        RuleFor(x => x.ShippingLabelUrls)
            .NotEmpty().WithMessage("Shipping label URLs are required.");
    }
}