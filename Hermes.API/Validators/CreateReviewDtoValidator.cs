using FluentValidation;
using Hermes.Application.DTOs;

namespace Hermes.API.Validators;

public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewDtoValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.ReviewText)
            .NotEmpty().WithMessage("Review text is required.")
            .MaximumLength(500).WithMessage("Review text cannot exceed 500 characters.");
    }
}