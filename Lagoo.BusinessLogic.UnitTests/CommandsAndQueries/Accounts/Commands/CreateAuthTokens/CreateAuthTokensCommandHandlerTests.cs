using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using Lagoo.BusinessLogic.UnitTests.Common.Helpers;
using Lagoo.Domain.Entities;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;

[TestFixture]
public class CreateAuthTokensCommandHandlerTests : AccountTestsBase
{
    [SetUp]
    public void SetUp()
    {
        Context.RefreshTokens = TestHelpers.MockDbSet(Array.Empty<RefreshToken>());
    }

    [Test]
    public async Task Handle_CommandContainsDefaultUserAndAndDeviceId_ShouldReturnAuthTokenDataForExistingDevice()
    {
        Context.RefreshTokens = TestHelpers.MockDbSet(DefaultRefreshToken);

        var command = GenerateCommandWithValidDefaultData(DefaultDeviceId);

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.AreEqual(DefaultAccessTokenValue, result.AccessToken);
        Assert.AreEqual(DefaultAccessTokenExpirationDate, result.AccessTokenExpiresAt);
        Assert.AreEqual(DefaultRefreshTokenValue, result.RefreshTokenValue);
        Assert.AreEqual(UpdatedDefaultRefreshToken.ExpiresAt, result.RefreshTokenExpiresAt);
        Assert.AreEqual(DefaultDeviceId, result.DeviceId);
    }

    [Test]
    public async Task Handle_CommandContainsOnlyDefaultUser_ShouldReturnAuthTokenDataForNewDevice()
    {
        JwtAuthService.GenerateRefreshToken(DefaultUser, default).ReturnsForAnyArgs(DefaultNewRefreshToken);
        
        var command = GenerateCommandWithValidDefaultData();

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        AssertAuthenticationDataDtoContainsDefaultData(result);
        Assert.AreEqual(NewDeviceId, result.DeviceId);
    }
    
    [Test]
    public async Task Handle_CommandContainsNewUserAndDeviceId_ShouldReturnAuthTokenDataForNewUserAndExistingDevice()
    {
        JwtAuthService.GenerateRefreshToken(DefaultUser, default).ReturnsForAnyArgs(DefaultRefreshToken);
        
        var user = new AppUser { Id = Guid.NewGuid() };
        
        Context.RefreshTokens.Add(new RefreshToken
        {
            DeviceId = DefaultDeviceId,
            Owner = user,
            OwnerId = user.Id
        });

        var command = GenerateCommandWithValidDefaultData(DefaultDeviceId);

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        AssertAuthenticationDataDtoContainsDefaultData(result);
        Assert.AreEqual(DefaultDeviceId, result.DeviceId);
    }

    private CreateAuthTokensCommandHandler CreateHandler() => new(Context, JwtAuthService);

    private CreateAuthTokensCommand GenerateCommandWithValidDefaultData(Guid? deviceId = null) => new(DefaultUser)
    {
        DeviceId = deviceId
    };
}