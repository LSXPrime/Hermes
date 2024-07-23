using FluentValidation;
using Hermes.Application.DTOs;

namespace Hermes.API.Validators;

public class OrderPreviewDtoValidator : AbstractValidator<OrderPreviewDto>
{
    public OrderPreviewDtoValidator()
    {
        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Shipping address is required.")
            .SetValidator(new AddressDtoValidator());

        RuleFor(x => x.BillingAddress)
            .NotNull().WithMessage("Billing address is required.")
            .SetValidator(new AddressDtoValidator()); 

        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemDtoValidator());
    }
}