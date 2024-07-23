using FluentValidation;
using Hermes.Application.DTOs;

namespace Hermes.API.Validators;

public class JwtTokenDtoValidator : AbstractValidator<JwtTokenDto>
{
    public JwtTokenDtoValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty().WithMessage("Access token is required.");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}