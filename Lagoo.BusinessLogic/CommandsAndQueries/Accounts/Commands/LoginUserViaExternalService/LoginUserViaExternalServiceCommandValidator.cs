using FluentValidation;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUserViaExternalService;

public class LoginUserViaExternalServiceCommandValidator : AbstractValidator<LoginUserViaExternalServiceCommand>
{
    public LoginUserViaExternalServiceCommandValidator()
    {
        RuleFor(luc => luc.ExternalAuthService)
            .NotNull().WithMessage(AccountResources.ExternalAuthServiceNotSpecified)
            .IsInEnum().WithMessage(AccountResources.InvalidExternalAuthService);

        RuleFor(luc => luc.ExternalAuthServiceAccessToken)
            .NotEmpty().WithMessage(AccountResources.AccessTokenIsEmpty);
    }
}