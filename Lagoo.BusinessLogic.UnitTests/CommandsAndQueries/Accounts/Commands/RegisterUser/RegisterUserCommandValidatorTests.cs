using FluentValidation.Results;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RegisterUser;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using Lagoo.BusinessLogic.UnitTests.Common.Helpers;
using Lagoo.Domain.Enums;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.RegisterUser;

/// <summary>
///   Tests for <see cref="RegisterUserCommandValidator"/>
/// </summary>
[TestFixture]
public class RegisterUserCommandValidatorTests : AccountTestsBase
{
    [Test]
    public void Validate_CommandContainsValidDataForRegisteringUserWithLocalAccount_ShouldReturnValidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommand(password: ValidPassword, confirmPassword: ValidPassword));
        
        Assert.IsTrue(result.IsValid);
    }

    [Test]
    public void Validate_CommandContainsValidDataForRegisteringUserViaExternalAuthService_ShouldReturnValidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommand(externalAuthService: ExternalAuthService.Google,
            externalAuthServiceAccessToken: ExternalAuthServiceAccessToken));
        
        Assert.IsTrue(result.IsValid);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespaceFirstName_ShouldReturnInvalidResultOfValidation(string? firstName)
    {
        var result = PerformValidation(GenerateCommand(firstName, password: ValidPassword, confirmPassword: ValidPassword));
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithTooLongFirstName_ShouldReturnInvalidResultOfValidation()
    {
        var tooLongFirstName = StringHelpers.GenerateRandomString(257);

        var result = PerformValidation(GenerateCommand(
            tooLongFirstName, password: ValidPassword, confirmPassword: ValidPassword));
        
        Assert.IsFalse(result.IsValid);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespaceLastName_ShouldReturnInvalidResultOfValidation(string? lastName)
    {
        var result = PerformValidation(GenerateCommand(lastName: lastName, password: ValidPassword, confirmPassword: ValidPassword));
        
        Assert.IsFalse(result.IsValid);
    }
    
    [Test]
    public void Validate_CommandWithTooLongLastName_ShouldReturnInvalidResultOfValidation()
    {
        var tooLongLastName = StringHelpers.GenerateRandomString(257);

        var result = PerformValidation(GenerateCommand(
            tooLongLastName, password: ValidPassword, confirmPassword: ValidPassword));
        
        Assert.IsFalse(result.IsValid);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("invalid_email.gmail")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespaceEmail_ShouldReturnInvalidResultOfValidation(string? email)
    {
        var result = PerformValidation(GenerateCommand(email: email, password: ValidPassword, confirmPassword: ValidPassword));
        
        Assert.IsFalse(result.IsValid);
    }
    
    [Test]
    public void Validate_CommandWithTooLongEmail_ShouldReturnInvalidResultOfValidation()
    {
        var tooLongEmail = StringHelpers.GenerateRandomString(231) + DefaultUserEmail;

        var result = PerformValidation(GenerateCommand(
             tooLongEmail, password: ValidPassword, confirmPassword: ValidPassword));
        
        Assert.IsFalse(result.IsValid);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("qQ1=")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespaceOrTooShortPassword_ShouldReturnInvalidResultOfValidation(string? password)
    {
        var result = PerformValidation(GenerateCommand(password: password, confirmPassword: password));
        
        Assert.IsFalse(result.IsValid);
    }
    
    [TestCase("wwwwwwww1234=+")]
    [TestCase("WWWWWWWW1234=+")]
    [TestCase("wwwwWWWW===+++")]
    [TestCase("wwwwWWWW123456")]
    public void Validate_CommandWithLowercaseOrUppercaseOrNotAlphanumericOrWithoutSpecialCharactersPassword_ShouldReturnInvalidResultOfValidation(string? password)
    {
        var result = PerformValidation(GenerateCommand(password: password, confirmPassword: password));
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithInvalidConfirmPassword_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommand(password: ValidPassword, confirmPassword: "different_password"));
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithOmittedExternalAuthServiceForRegistrationViaExternalAuth_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommand(externalAuthServiceAccessToken: ExternalAuthServiceAccessToken));
        
        Assert.IsFalse(result.IsValid);
    }
    
    [Test]
    public void Validate_CommandWithInvalidExternalAuthServiceForRegistrationViaExternalAuth_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommand(externalAuthService: (ExternalAuthService) 100, externalAuthServiceAccessToken: ExternalAuthServiceAccessToken));
        
        Assert.IsFalse(result.IsValid);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespaceExternalAuthServiceAccessTokenForRegistrationViaExternalAuth_ShouldReturnInvalidResultOfValidation(string? accessToken)
    {
        var result = PerformValidation(GenerateCommand(externalAuthService: ExternalAuthService.Google,
            externalAuthServiceAccessToken: accessToken));
        
        Assert.IsFalse(result.IsValid);
    }

    private RegisterUserCommandValidator CreateValidator() => new();

    private ValidationResult PerformValidation(RegisterUserCommand command)
    {
        var validator = CreateValidator();

        return validator.Validate(command);
    }

    private RegisterUserCommand GenerateCommand(string firstName = "Brandon", string lastName = "Harvinson",
        string email = "smth@gmail.com", string? password = null, string? confirmPassword = null,
        ExternalAuthService? externalAuthService = null, string? externalAuthServiceAccessToken = null) => new()
    {
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        Password = password,
        ConfirmPassword = confirmPassword,
        ExternalAuthService = externalAuthService,
        ExternalAuthServiceAccessToken = externalAuthServiceAccessToken
    };
}