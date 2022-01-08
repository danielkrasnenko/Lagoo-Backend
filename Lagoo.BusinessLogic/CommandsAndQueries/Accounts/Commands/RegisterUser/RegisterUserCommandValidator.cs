using System.Text.RegularExpressions;
using FluentValidation;
using Lagoo.BusinessLogic.Common.Constants;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Microsoft.Extensions.Localization;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private const short FirstNameMaxLength = 256;

    private const short LastNameMaxLength = 256;

    private const short EmailMaxLength = 256;

    private const short PasswordMinLength = 5;

    public RegisterUserCommandValidator(IStringLocalizer<AccountResources> accountLocalizer)
    {
        RuleFor(ruc => ruc.FirstName)
            .NotEmpty().WithMessage(accountLocalizer["FirstNameIsEmpty"])
            .MaximumLength(FirstNameMaxLength).WithMessage(accountLocalizer["FirstNameIsTooLong", FirstNameMaxLength]);

        RuleFor(ruc => ruc.LastName)
            .NotEmpty().WithMessage(accountLocalizer["LastNameIsEmpty"])
            .MaximumLength(LastNameMaxLength).WithMessage(accountLocalizer["LastNameIsTooLong", LastNameMaxLength]);

        RuleFor(ruc => ruc.Email)
            .NotEmpty().WithMessage(accountLocalizer["EmailIsEmpty"])
            .MaximumLength(EmailMaxLength).WithMessage(accountLocalizer["EmailIsTooLong", EmailMaxLength])
            .Must(email => email.IsEmail()).WithMessage(accountLocalizer["EmailIsInvalid"]);

        RuleFor(ruc => ruc.Password)
            .NotEmpty().WithMessage(accountLocalizer["PasswordIsEmpty"])
            .MinimumLength(PasswordMinLength).WithMessage(accountLocalizer["PasswordIsTooShort", PasswordMinLength])
            .Must(p => p.Any(char.IsUpper)).WithMessage(accountLocalizer["PasswordWithoutUppercaseLetters"])
            .Must(p => p.Any(char.IsLower)).WithMessage(accountLocalizer["PasswordWithoutLowercaseLetters"])
            .Must(p => p.Any(char.IsNumber)).WithMessage(accountLocalizer["PasswordWithoutNumericCharacters"])
            .Must(p => Regex.IsMatch(p, RegexConstants.SpecialCharacters)).WithMessage(accountLocalizer["PasswordWithoutSpecialCharacters"])
            .When(ruc =>
                ruc.ExternalAuthService is null && ruc.ExternalAuthServiceAccessToken is null);
        
        RuleFor(ruc => ruc.ConfirmPassword)
            .Equal(ruc => ruc.Password).WithMessage(accountLocalizer["ConfirmPasswordDoesNotMatchPassword"])
            .When(ruc => ruc.Password is not null);

        RuleFor(ruc => ruc.ExternalAuthService)
            .NotNull().WithMessage(accountLocalizer["ExternalAuthServiceNotSpecified"])
            .IsInEnum().WithMessage(accountLocalizer["InvalidExternalAuthService"])
            .When(ruc => ruc.ExternalAuthServiceAccessToken is not null);

        RuleFor(ruc => ruc.ExternalAuthServiceAccessToken)
            .NotEmpty().WithMessage(accountLocalizer["AccessTokenIsEmpty"])
            .When(ruc => ruc.ExternalAuthService is not null);
    }
}