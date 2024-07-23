using FluentValidation;
using Hermes.Application.DTOs;

namespace Hermes.API.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Shipping address is required.")
            .SetValidator(new AddressDtoValidator());

        RuleFor(x => x.BillingAddress)
            .NotNull().WithMessage("Billing address is required.")
            .SetValidator(new AddressDtoValidator()); 

        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("Payment method is required.");

        RuleForEach(x => x.OrderItems)
            .SetValidator(new OrderItemDtoValidator());
    }
}