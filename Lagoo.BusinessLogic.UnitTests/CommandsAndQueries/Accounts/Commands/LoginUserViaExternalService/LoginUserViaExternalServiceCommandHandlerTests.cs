using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUserViaExternalService;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.LoginUserViaExternalService;

/// <summary>
///   Tests for <see cref="LoginUserViaExternalServiceCommandHandler"/>
/// </summary>
[TestFixture]
public class LoginUserViaExternalServiceCommandHandlerTests : AccountTestsBase
{
    [SetUp]
    public void SetUp()
    {
        SetServicesDefaultBehaviour();
    }

    [Test]
    public async Task Handle_CommandContainsValidDataForLoginAndDeviceId_ShouldReturnAuthTokenDataForExistingDevice()
    {
        var command = GenerateCommandWithValidDefaultData(deviceId: DefaultDeviceId);

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        AssertAuthenticationDataDtoContainsDefaultData(result);
        Assert.AreEqual(DefaultDeviceId, result.DeviceId);
    }
    
    [Test]
    public async Task Handle_CommandContainsOnlyValidDataForLogin_ShouldReturnAuthTokenDataForNewDevice()
    {
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser)).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(NewDeviceId));
        
        var command = GenerateCommandWithValidDefaultData();

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        AssertAuthenticationDataDtoContainsDefaultData(result);
        Assert.AreEqual(NewDeviceId, result.DeviceId);
    }

    [Test]
    public void Handle_UserManagerCannotFindUserByLogin_ShouldThrowBadRequestException()
    {
        AppUser? user = null;
        UserManager.FindByLoginAsync("", default).ReturnsForAnyArgs(user);
        
        var command = GenerateCommandWithValidDefaultData();

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }
    
    private LoginUserViaExternalServiceCommandHandler CreateHandler() => new(ExternalAuthServicesManager, UserManager, Mediator);

    private LoginUserViaExternalServiceCommand GenerateCommandWithValidDefaultData(
        ExternalAuthService externalAuthService = ExternalAuthService.Google,
        string accessToken = DefaultAccessToken, Guid? deviceId = null) => new()
    {
        ExternalAuthService = externalAuthService,
        ExternalAuthServiceAccessToken = accessToken,
        DeviceId = deviceId
    };
    
    private void SetServicesDefaultBehaviour()
    {
        UserManager.FindByLoginAsync("", default).ReturnsForAnyArgs(DefaultUser);
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser)).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(DefaultDeviceId));
    }
}