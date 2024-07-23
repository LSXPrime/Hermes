using FluentValidation;
using Hermes.Application.DTOs;

namespace Hermes.API.Validators;

public class ProductVariantDtoValidator : AbstractValidator<ProductVariantDto>
{
    public ProductVariantDtoValidator()
    {
        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative.");

        RuleForEach(x => x.Options)
            .SetValidator(new ProductVariantOptionDtoValidator());
    }
}