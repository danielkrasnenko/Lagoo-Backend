using FluentValidation.Results;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RefreshAccessToken;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.RefreshAccessToken;

/// <summary>
///   Tests for <see cref="RefreshAccessTokenCommandValidator"/>
/// </summary>
[TestFixture]
public class RefreshAccessTokenCommandValidatorTests : AccountTestsBase
{
    [Test]
    public void Validate_CommandContainsValidData_ShouldReturnValidResultOfValidation()
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData());
        
        Assert.IsTrue(result.IsValid);
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespaceAccessToken_ShouldReturnInvalidResultOfValidation(string? accessToken)
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData(accessToken));

        Assert.IsFalse(result.IsValid);
    }
    
    [TestCase(null)]
    [TestCase("")]
    [TestCase("   ")]
    public void Validate_CommandWithOmittedOrEmptyOrWhitespaceRefreshTokenValue_ShouldReturnInvalidResultOfValidation(string? refreshTokenValue)
    {
        var result = PerformValidation(GenerateCommandWithValidDefaultData(refreshTokenValue: refreshTokenValue));
        
        Assert.IsFalse(result.IsValid);
    }
    
    private RefreshAccessTokenCommandValidator CreateValidator() => new();
    
    private ValidationResult PerformValidation(RefreshAccessTokenCommand command)
    {
        var validator = CreateValidator();

        return validator.Validate(command);
    }

    private RefreshAccessTokenCommand GenerateCommandWithValidDefaultData(string accessToken = DefaultAccessToken,
        string refreshTokenValue = DefaultRefreshTokenValue) => new()
    {
        AccessToken = accessToken,
        RefreshTokenValue = refreshTokenValue
    };
}