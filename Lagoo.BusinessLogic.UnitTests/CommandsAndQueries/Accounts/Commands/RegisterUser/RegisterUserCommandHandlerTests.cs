using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RegisterUser;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using Lagoo.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.RegisterUser;

/// <summary>
///   Tests for <see cref="RegisterUserCommandHandler"/>
/// </summary>
[TestFixture]
public class RegisterUserCommandHandlerTests : AccountTestsBase
{
    [SetUp]
    public void SetUp()
    {
        SetServicesDefaultBehaviour();
    }
    
    [Test]
    public async Task Handle_CommandContainsValidDataForRegisteringUserWithLocalAccountAndDeviceId_ShouldReturnAuthTokenDataForExistingDevice()
    {
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser) { DeviceId = DefaultDeviceId }).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(DefaultDeviceId));
        
        var command = GenerateCommandWithDefaultUserData("password", "password", DefaultDeviceId);

        var handler = CreateHandler();
        
        var result = await handler.Handle(command, CancellationToken.None);

        AssertAuthenticationDataDtoContainsDefaultData(result);
        Assert.AreEqual(DefaultDeviceId, result.DeviceId);
    }
    
    [Test]
    public async Task Handle_CommandContainsOnlyValidDataForRegisteringUserWithLocalAccount_ShouldReturnAuthTokenDataForNewDevice()
    {
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser)).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(NewDeviceId));
        
        var command = GenerateCommandWithDefaultUserData("password", "password");

        var handler = CreateHandler();
        
        var result = await handler.Handle(command, CancellationToken.None);

        AssertAuthenticationDataDtoContainsDefaultData(result);
        Assert.AreEqual(NewDeviceId, result.DeviceId);
    }

    [Test]
    public async Task Handle_CommandContainsValidDataForRegisteringUserViaExternalAuthServiceAndDeviceId_ShouldReturnAuthTokenDataForExistingDevice()
    {
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser) { DeviceId = DefaultDeviceId }).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(DefaultDeviceId));
        
        var command = GenerateCommandWithDefaultUserData(deviceId: DefaultDeviceId,
            externalAuthService: ExternalAuthService.Google, externalAuthServiceAccessToken: "access_token");

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        AssertAuthenticationDataDtoContainsDefaultData(result);
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
        
        AssertAuthenticationDataDtoContainsDefaultData(result);
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

    private RegisterUserCommandHandler CreateHandler() => new(UserRepository, Mediator, ExternalAuthServicesManager);

    private void SetServicesDefaultBehaviour()
    {
        UserRepository.CreateAsync(DefaultUser).ReturnsForAnyArgs(IdentityResult.Success);
        UserRepository.CreateAsync(DefaultUser, "").ReturnsForAnyArgs(IdentityResult.Success);
        UserRepository.AddToRoleAsync(DefaultUser, default).ReturnsForAnyArgs(IdentityResult.Success);
        ExternalAuthServicesManager.BindUserAsync(DefaultUser, default, "").ReturnsForAnyArgs(IdentityResult.Success);
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser)).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(DefaultDeviceId));
    }

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