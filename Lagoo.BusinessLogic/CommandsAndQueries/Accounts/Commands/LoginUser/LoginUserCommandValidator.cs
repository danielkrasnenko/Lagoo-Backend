using FluentValidation;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Microsoft.Extensions.Localization;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator(IStringLocalizer<AccountResources> accountLocalizer)
    {
        RuleFor(luc => luc.Email)
            .NotEmpty().WithMessage(accountLocalizer["EmailIsEmpty"])
            .Must(email => email.IsEmail()).WithMessage(accountLocalizer["EmailIsInvalid"]);

        RuleFor(luc => luc.Password)
            .NotEmpty().WithMessage(accountLocalizer["PasswordIsEmpty"]);
        
        RuleFor(luc => luc.DeviceId)
            .NotEmpty().WithMessage(accountLocalizer["DeviceIdIsNotProvided"]);
    }
}