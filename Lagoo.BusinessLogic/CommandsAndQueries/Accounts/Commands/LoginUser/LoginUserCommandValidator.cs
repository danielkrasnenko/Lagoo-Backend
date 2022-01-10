using FluentValidation;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(luc => luc.Email)
            .NotEmpty().WithMessage(AccountResources.EmailIsEmpty)
            .Must(email => email.IsEmail()).WithMessage(AccountResources.EmailIsInvalid);

        RuleFor(luc => luc.Password)
            .NotEmpty().WithMessage(AccountResources.PasswordIsEmpty);
    }
}