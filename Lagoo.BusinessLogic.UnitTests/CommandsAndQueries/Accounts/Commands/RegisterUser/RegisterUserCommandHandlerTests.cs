using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RegisterUser;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using Lagoo.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.RegisterUser;

[TestFixture]
public class RegisterUserCommandHandlerTests : AccountTestsBase
{
    [SetUp]
    public void SetUp()
    {
        SetServicesDefaultBehaviour();
    }
    
    [Test]
    public async Task Handle_CommandContainsDeviceIdAndValidDataForRegisteringUserWithLocalAccount_ShouldReturnAuthTokenDataForExistingDevice()
    {
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser) { DeviceId = DefaultDeviceId }).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(DefaultDeviceId));
        
        var command = GenerateCommandWithDefaultUserData("password", "password", DefaultDeviceId);

        var handler = CreateHandler();
        
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.AreEqual(DefaultAccessTokenValue, result.AccessToken);
        Assert.AreEqual(DefaultAccessTokenExpirationDate, result.AccessTokenExpiresAt);
        Assert.AreEqual(DefaultRefreshTokenValue, result.RefreshTokenValue);
        Assert.AreEqual(DefaultRefreshToken.ExpiresAt, result.RefreshTokenExpiresAt);
        Assert.AreEqual(DefaultDeviceId, result.DeviceId);
    }
    
    [Test]
    public async Task Handle_CommandContainsOnlyValidDataForRegisteringUserWithLocalAccount_ShouldReturnAuthTokenDataForNewDevice()
    {
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser)).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(NewDeviceId));
        
        var command = GenerateCommandWithDefaultUserData("password", "password");

        var handler = CreateHandler();
        
        var result = await handler.Handle(command, CancellationToken.None);

        Assert.AreEqual(DefaultAccessTokenValue, result.AccessToken);
        Assert.AreEqual(DefaultAccessTokenExpirationDate, result.AccessTokenExpiresAt);
        Assert.AreEqual(DefaultRefreshTokenValue, result.RefreshTokenValue);
        Assert.AreEqual(DefaultRefreshToken.ExpiresAt, result.RefreshTokenExpiresAt);
        Assert.AreEqual(NewDeviceId, result.DeviceId);
    }

    [Test]
    public async Task Handle_CommandContainsDeviceIdAndValidDataForRegisteringUserViaExternalAuthService_ShouldReturnAuthTokenDataForExistingDevice()
    {
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser) { DeviceId = DefaultDeviceId }).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(DefaultDeviceId));
        
        var command = GenerateCommandWithDefaultUserData(deviceId: DefaultDeviceId,
            externalAuthService: ExternalAuthService.Google, externalAuthServiceAccessToken: "access_token");

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.AreEqual(DefaultAccessTokenValue, result.AccessToken);
        Assert.AreEqual(DefaultAccessTokenExpirationDate, result.AccessTokenExpiresAt);
        Assert.AreEqual(DefaultRefreshTokenValue, result.RefreshTokenValue);
        Assert.AreEqual(DefaultRefreshToken.ExpiresAt, result.RefreshTokenExpiresAt);
        Assert.AreEqual(DefaultDeviceId, result.DeviceId);
    }
    
    [Test]
    public async Task Handle_CommandContainsOnlyValidDataForRegisteringUserViaExternalAuthService_ShouldReturnAuthTokenDataForNewDevice()
    {
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser)).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(NewDeviceId));
        
        var command = GenerateCommandWithDefaultUserData(externalAuthService: ExternalAuthService.Google,
            externalAuthServiceAccessToken: "access_token");

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.AreEqual(DefaultAccessTokenValue, result.AccessToken);
        Assert.AreEqual(DefaultAccessTokenExpirationDate, result.AccessTokenExpiresAt);
        Assert.AreEqual(DefaultRefreshTokenValue, result.RefreshTokenValue);
        Assert.AreEqual(DefaultRefreshToken.ExpiresAt, result.RefreshTokenExpiresAt);
        Assert.AreEqual(NewDeviceId, result.DeviceId);
    }

    [Test]
    public void Handle_UserManagerCannotCreateUserWithLocalAccount_ShouldThrowBadRequestException()
    {
        UserManager.CreateAsync(DefaultUser, "").ReturnsForAnyArgs(IdentityResult.Failed());
        
        var command = GenerateCommandWithDefaultUserData("password", "password");

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }
    
    [Test]
    public void Handle_UserManagerCannotCreateUserWithExternalAccount_ShouldThrowBadRequestException()
    {
        UserManager.CreateAsync(DefaultUser).ReturnsForAnyArgs(IdentityResult.Failed());

        var command = GenerateCommandWithDefaultUserData(externalAuthService: ExternalAuthService.Google,
            externalAuthServiceAccessToken: "access_token");

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_ExternalAuthServicesManagerCannotBindUser_ShouldThrowBadRequestException()
    {
        ExternalAuthServicesManager.BindUserAsync(DefaultUser, default, "").ReturnsForAnyArgs(IdentityResult.Failed());
        
        var command = GenerateCommandWithDefaultUserData(externalAuthService: ExternalAuthService.Google,
            externalAuthServiceAccessToken: "access_token");

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_UserManagerCannotAddARoleToUser_ShouldThrowBadRequestException()
    {
        UserManager.AddToRoleAsync(DefaultUser, default).ReturnsForAnyArgs(IdentityResult.Failed());
        
        var command = GenerateCommandWithDefaultUserData("password", "password");

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }

    private RegisterUserCommandHandler CreateHandler() => new(UserManager, Mediator, ExternalAuthServicesManager);

    private void SetServicesDefaultBehaviour()
    {
        UserManager.CreateAsync(DefaultUser).ReturnsForAnyArgs(IdentityResult.Success);
        UserManager.CreateAsync(DefaultUser, "").ReturnsForAnyArgs(IdentityResult.Success);
        UserManager.AddToRoleAsync(DefaultUser, default).ReturnsForAnyArgs(IdentityResult.Success);
        ExternalAuthServicesManager.BindUserAsync(DefaultUser, default, "").ReturnsForAnyArgs(IdentityResult.Success);
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser)).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(DefaultDeviceId));
    }

    private AuthenticationDataDto GenerateDefaultAuthDataDto(Guid deviceId) => new()
    {
        AccessToken = DefaultAccessTokenValue,
        AccessTokenExpiresAt = DefaultAccessTokenExpirationDate,
        RefreshTokenValue = DefaultRefreshTokenValue,
        RefreshTokenExpiresAt = DefaultRefreshToken.ExpiresAt,
        DeviceId = deviceId
    };

    private RegisterUserCommand GenerateCommandWithDefaultUserData(string? password = null, string? confirmPassword = null,
        Guid? deviceId = null, ExternalAuthService? externalAuthService = null, string? externalAuthServiceAccessToken = null) => new()
    {
        FirstName = DefaultUser.FirstName,
        LastName = DefaultUser.LastName,
        Email = DefaultUserEmail,
        Password = password,
        ConfirmPassword = confirmPassword,
        DeviceId = deviceId,
        ExternalAuthService = externalAuthService,
        ExternalAuthServiceAccessToken = externalAuthServiceAccessToken
    };
}