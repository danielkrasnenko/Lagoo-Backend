using FluentValidation.Results;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUserViaExternalService;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using Lagoo.Domain.Enums;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.LoginUserViaExternalService;

[TestFixture]
public class LoginUserViaExternalServiceCommandValidatorTests : AccountTestsBase
{
    [Test]
    public void Validate_CommandContainsValidData_ShouldReturnValidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData());
        
        Assert.IsTrue(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithDefaultValueForExternalAuthService_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData(0));
        
        Assert.IsFalse(result.IsValid);
    }

    [Test]
    public void Validate_CommandWithInvalidExternalAuthService_ShouldReturnInvalidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData((ExternalAuthService) 100));
        
        Assert.IsFalse(result.IsValid);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate(string? accessToken)
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData(accessToken: accessToken));
        
        Assert.IsFalse(result.IsValid);
    }
    
    private LoginUserViaExternalServiceCommandValidator CreateValidator() => new();
    
    private ValidationResult PerformValidation(LoginUserViaExternalServiceCommand command)
    {
        var validator = CreateValidator();

        return validator.Validate(command);
    }

    private LoginUserViaExternalServiceCommand GenerateCommandWithValidDefaultData(
        ExternalAuthService externalAuthService = ExternalAuthService.Google,
        string accessToken = DefaultAccessTokenValue) => new()
    {
        ExternalAuthService = externalAuthService,
        ExternalAuthServiceAccessToken = accessToken
    };
}