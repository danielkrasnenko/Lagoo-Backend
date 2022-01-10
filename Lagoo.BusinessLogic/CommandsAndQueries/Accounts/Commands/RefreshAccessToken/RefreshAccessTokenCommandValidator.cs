using FluentValidation;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RefreshAccessToken;

public class RefreshAccessTokenCommandValidator : AbstractValidator<RefreshAccessTokenCommand>
{
    public RefreshAccessTokenCommandValidator()
    {
        RuleFor(ratc => ratc.AccessToken)
            .NotEmpty().WithMessage(AccountResources.AccessTokenIsEmpty);

        RuleFor(ratc => ratc.RefreshTokenValue)
            .NotEmpty().WithMessage(AccountResources.RefreshTokenValueIsEmpty);
    }
}