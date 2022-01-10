using System.Text.RegularExpressions;
using FluentValidation;
using Lagoo.BusinessLogic.Common.Constants;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private const short FirstNameMaxLength = 256;

    private const short LastNameMaxLength = 256;

    private const short EmailMaxLength = 256;

    private const short PasswordMinLength = 5;

    public RegisterUserCommandValidator()
    {
        RuleFor(ruc => ruc.FirstName)
            .NotEmpty().WithMessage(AccountResources.FirstNameIsEmpty)
            .MaximumLength(FirstNameMaxLength).WithMessage(string.Format(AccountResources.FirstNameIsTooLong, FirstNameMaxLength));

        RuleFor(ruc => ruc.LastName)
            .NotEmpty().WithMessage(AccountResources.LastNameIsEmpty)
            .MaximumLength(LastNameMaxLength).WithMessage(string.Format(AccountResources.LastNameIsTooLong, LastNameMaxLength));

        RuleFor(ruc => ruc.Email)
            .NotEmpty().WithMessage(AccountResources.EmailIsEmpty)
            .MaximumLength(EmailMaxLength).WithMessage(string.Format(AccountResources.EmailIsTooLong, EmailMaxLength))
            .Must(email => email.IsEmail()).WithMessage(AccountResources.EmailIsInvalid);

        RuleFor(ruc => ruc.Password)
            .NotEmpty().WithMessage(AccountResources.PasswordIsEmpty)
            .MinimumLength(PasswordMinLength).WithMessage(string.Format(AccountResources.PasswordIsTooShort, PasswordMinLength))
            .Must(p => p.Any(char.IsUpper)).WithMessage(AccountResources.PasswordWithoutUppercaseLetters)
            .Must(p => p.Any(char.IsLower)).WithMessage(AccountResources.PasswordWithoutLowercaseLetters)
            .Must(p => p.Any(char.IsNumber)).WithMessage(AccountResources.PasswordWithoutNumericCharacters)
            .Must(p => Regex.IsMatch(p, RegexConstants.SpecialCharacters)).WithMessage(AccountResources.PasswordWithoutSpecialCharacters)
            .When(ruc =>
                ruc.ExternalAuthService is null && ruc.ExternalAuthServiceAccessToken is null);
        
        RuleFor(ruc => ruc.ConfirmPassword)
            .Equal(ruc => ruc.Password).WithMessage(AccountResources.ConfirmPasswordDoesNotMatchPassword)
            .When(ruc => ruc.Password is not null);

        RuleFor(ruc => ruc.ExternalAuthService)
            .NotNull().WithMessage(AccountResources.ExternalAuthServiceNotSpecified)
            .IsInEnum().WithMessage(AccountResources.InvalidExternalAuthService)
            .When(ruc => ruc.ExternalAuthServiceAccessToken is not null);

        RuleFor(ruc => ruc.ExternalAuthServiceAccessToken)
            .NotEmpty().WithMessage(AccountResources.AccessTokenIsEmpty)
            .When(ruc => ruc.ExternalAuthService is not null);
    }
}