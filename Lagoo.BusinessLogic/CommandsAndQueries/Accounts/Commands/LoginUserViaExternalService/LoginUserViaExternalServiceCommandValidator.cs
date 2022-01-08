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

        RuleFor(luc => luc.ExternalAuthServiceAccessToken)
            .NotEmpty().WithMessage(accountLocalizer["AccessTokenIsEmpty"]);
    }
}