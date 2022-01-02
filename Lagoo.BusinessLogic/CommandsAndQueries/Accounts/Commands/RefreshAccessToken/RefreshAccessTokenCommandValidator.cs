using FluentValidation;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Microsoft.Extensions.Localization;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RefreshAccessToken;

public class RefreshAccessTokenCommandValidator : AbstractValidator<RefreshAccessTokenCommand>
{
    public RefreshAccessTokenCommandValidator(IStringLocalizer<AccountResources> accountLocalizer)
    {
        RuleFor(ratc => ratc.AccessToken)
            .NotEmpty().WithMessage(accountLocalizer["AccessTokenIsEmpty"]);

        RuleFor(ratc => ratc.RefreshTokenValue)
            .NotEmpty().WithMessage(accountLocalizer["RefreshTokenValueIsEmpty"]);
    }
}