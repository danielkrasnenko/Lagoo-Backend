using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using Lagoo.Domain.Entities;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.LoginUser;

public class LoginUserCommandHandlerTests : AccountTestsBase
{
    [SetUp]
    public void SetUp()
    {
        SetServicesDefaultBehaviour();
    }

    [Test]
    public async Task Handle_CommandContainsDeviceIdAndValidData_ShouldReturnAuthTokenDataForExistingDevice()
    {
        var command = new LoginUserCommand
        {
            Email = DefaultUserEmail,
            Password = ValidPassword,
            DeviceId = DefaultDeviceId
        };

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.AreEqual(DefaultAccessTokenValue, result.AccessToken);
        Assert.AreEqual(DefaultAccessTokenExpirationDate, result.AccessTokenExpiresAt);
        Assert.AreEqual(DefaultRefreshTokenValue, result.RefreshTokenValue);
        Assert.AreEqual(DefaultRefreshToken.ExpiresAt, result.RefreshTokenExpiresAt);
        Assert.AreEqual(DefaultDeviceId, result.DeviceId);
    }

    [Test]
    public async Task Handle_CommandContainsValidDataWithoutDeviceId_ShouldReturnAuthTokenDataForExistingDevice()
    {
        Mediator.Send(new CreateAuthTokensCommand(DefaultUser)).ReturnsForAnyArgs(GenerateDefaultAuthDataDto(NewDeviceId));
        
        var command = new LoginUserCommand
        {
            Email = DefaultUserEmail,
            Password = ValidPassword
        };

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.AreEqual(DefaultAccessTokenValue, result.AccessToken);
        Assert.AreEqual(DefaultAccessTokenExpirationDate, result.AccessTokenExpiresAt);
        Assert.AreEqual(DefaultRefreshTokenValue, result.RefreshTokenValue);
        Assert.AreEqual(DefaultRefreshToken.ExpiresAt, result.RefreshTokenExpiresAt);
        Assert.AreEqual(NewDeviceId, result.DeviceId);
    }

    [Test]
    public void Handle_UserIsNotRegistered_ShouldThrowBadRequestException()
    {
        AppUser? user = null;
        UserManager.FindByEmailAsync(default).ReturnsForAnyArgs(user);

        var command = new LoginUserCommand
        {
            Email = DefaultUserEmail,
            Password = ValidPassword
        };

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Test]
    public void Handle_CommandWithWrongPassword_ShouldThrowBadRequestException()
    {
        UserManager.CheckPasswordAsync(DefaultUser, default).ReturnsForAnyArgs(false);
        
        var command = new LoginUserCommand
        {
            Email = DefaultUserEmail,
            Password = ValidPassword
        };
        
        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }
    
    private LoginUserCommandHandler CreateHandler() => new(UserManager, Mediator);
    
    private void SetServicesDefaultBehaviour()
    {
        UserManager.FindByEmailAsync(default).ReturnsForAnyArgs(DefaultUser);
        UserManager.CheckPasswordAsync(DefaultUser, default).ReturnsForAnyArgs(true);
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
}