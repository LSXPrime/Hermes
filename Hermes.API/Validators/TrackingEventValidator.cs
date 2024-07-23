using FluentValidation;
using Hermes.Application.DTOs;

namespace Hermes.API.Validators;

public class TrackingEventValidator : AbstractValidator<TrackingEvent>
{
    public TrackingEventValidator()
    {
        RuleFor(x => x.Timestamp)
            .NotEmpty().WithMessage("Timestamp is required.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");
    }
}