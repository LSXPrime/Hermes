using FluentValidation;
using Hermes.Application.DTOs;

namespace Hermes.API.Validators;

public class ShippingRateValidator : AbstractValidator<ShippingRate>
{
    public ShippingRateValidator()
    {
        RuleFor(x => x.Carrier)
            .NotEmpty().WithMessage("Carrier is required.");

        RuleFor(x => x.ServiceName)
            .NotEmpty().WithMessage("Service name is required.");

        RuleFor(x => x.TotalRate)
            .GreaterThanOrEqualTo(0).WithMessage("Total rate must be non-negative.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.");

        RuleFor(x => x.EstimatedDeliveryDate)
            .NotEmpty().WithMessage("Estimated delivery date is required.");
    }
}