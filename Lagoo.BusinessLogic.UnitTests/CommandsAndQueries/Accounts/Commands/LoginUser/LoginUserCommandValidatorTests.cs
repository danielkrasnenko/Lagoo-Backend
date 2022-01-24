using FluentValidation.Results;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.LoginUser;

[TestFixture]
public class LoginUserCommandValidatorTests : AccountTestsBase
{
    [Test]
    public void Validate_CommandContainsValidData_ShouldReturnValidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommandWithValidData());
        
        Assert.IsTrue(result.IsValid);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    [TestCase("invalid_email.gmail")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespaceEmail_ShouldReturnInvalidResultOfValidation(string? email)
    {
        var result = PerformValidation(GenerateCommandWithValidData(email));
        
        Assert.IsFalse(result.IsValid);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespacePassword_ShouldReturnInvalidResultOfValidation(string? password)
    {
        var result = PerformValidation(GenerateCommandWithValidData(password: password));
        
        Assert.IsFalse(result.IsValid);
    }

    private LoginUserCommandValidator CreateValidator() => new();

    private ValidationResult PerformValidation(LoginUserCommand command)
    {
        var validator = CreateValidator();

        return validator.Validate(command);
    }

    private LoginUserCommand GenerateCommandWithValidData(string email = DefaultUserEmail,
        string password = ValidPassword) => new()
    {
        Email = email,
        Password = password
    };
}