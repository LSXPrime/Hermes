using FluentValidation;
using Hermes.Application.DTOs;

namespace Hermes.API.Validators;

public class UpdateProductVariantDtoValidator : AbstractValidator<UpdateProductVariantDto>
{
    public UpdateProductVariantDtoValidator()
    {
        RuleFor(x => x.SKU)
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative.");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative.");

        RuleForEach(x => x.Options)
            .SetValidator(new ProductVariantOptionDtoValidator());
    }
}