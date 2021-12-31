using FluentValidation;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Microsoft.Extensions.Localization;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUserViaExternalService;

public class LoginUserViaExternalServiceCommandValidator : AbstractValidator<LoginUserViaExternalServiceCommand>
{
    public LoginUserViaExternalServiceCommandValidator(IStringLocalizer<AccountResources> accountLocalizer)
    {
        RuleFor(luc => luc.ExternalAuthService)
            .NotNull().WithMessage(accountLocalizer["ExternalAuthServiceNotSpecified"])
            .IsInEnum().WithMessage(accountLocalizer["InvalidExternalAuthService"]);

        RuleFor(luc => luc.ExternalServiceAccessToken)
            .NotEmpty().WithMessage(accountLocalizer["AccessTokenIsEmpty"]);
        
        RuleFor(luc => luc.RefreshTokenValue)
            .NotEmpty().WithMessage(accountLocalizer["RefreshTokenValueIsEmpty"])
            .When(luc => luc.RefreshTokenValue is not null);
    }
}