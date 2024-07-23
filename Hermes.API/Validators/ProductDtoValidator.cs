using FluentValidation;
using Hermes.Application.DTOs;

namespace Hermes.API.Validators;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Product description is required.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Category ID is required.");
            
        RuleForEach(x => x.Variants)
            .SetValidator(new ProductVariantDtoValidator());

        RuleForEach(x => x.Reviews)
            .SetValidator(new ReviewDtoValidator());
    }
}