using System;
using System.Threading;
using System.Threading.Tasks;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.Exceptions.Base;
using Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;
using Lagoo.Domain.Entities;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;

/// <summary>
///   Tests for <see cref="CreateAuthTokensCommandHandler"/>
/// </summary>
[TestFixture]
public class CreateAuthTokensCommandHandlerTests : AccountTestsBase
{
    [SetUp]
    public void SetUp()
    {
        RefreshTokenRepository.GetAsync(default, default, default).ReturnsForAnyArgs(null as object);
    }

    [Test]
    public async Task Handle_CommandContainsDefaultUserAndAndDeviceId_ShouldReturnAuthTokenDataForExistingDevice()
    {
        RefreshTokenRepository.ExistsAsync(DefaultUserId, DefaultDeviceId, CancellationToken.None).ReturnsForAnyArgs(true);
        RefreshTokenRepository
            .UpdateAsync(DefaultUserId, DefaultDeviceId, new UpdateRefreshTokenDto(), CancellationToken.None)
            .ReturnsForAnyArgs(DefaultReadRefreshTokenDto);

        var command = GenerateCommandWithValidDefaultData(DefaultDeviceId);

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        Assert.AreEqual(DefaultAccessToken, result.AccessToken);
        Assert.AreEqual(DefaultAccessTokenExpirationDate, result.AccessTokenExpiresAt);
        Assert.AreEqual(DefaultRefreshTokenValue, result.RefreshTokenValue);
        Assert.AreEqual(DefaultUpdateRefreshTokenDto.ExpiresAt, result.RefreshTokenExpiresAt);
        Assert.AreEqual(DefaultDeviceId, result.DeviceId);
    }

    [Test]
    public async Task Handle_CommandContainsOnlyDefaultUser_ShouldReturnAuthTokenDataForNewDevice()
    {
        RefreshTokenRepository.SaveAsync(new RefreshToken()).ReturnsForAnyArgs(DefaultReadRefreshTokenDto);
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
        RefreshTokenRepository.ExistsAsync(DefaultUserId, DefaultDeviceId, CancellationToken.None).Returns(true);
        RefreshTokenRepository.SaveAsync(new RefreshToken()).ReturnsForAnyArgs(DefaultReadRefreshTokenDto);
        JwtAuthService.GenerateRefreshToken(DefaultUser, default).ReturnsForAnyArgs(DefaultRefreshToken);

        var command = GenerateCommandWithValidDefaultData(DefaultDeviceId);

        var handler = CreateHandler();

        var result = await handler.Handle(command, CancellationToken.None);
        
        AssertAuthenticationDataDtoContainsDefaultData(result);
        Assert.AreEqual(DefaultDeviceId, result.DeviceId);
    }
    
    [Test]
    public void Handle_JwtAuthServiceCannotGenerateAccessToken_ShouldThrowBadRequestException()
    {
        JwtAuthService.GenerateAccessTokenAsync(DefaultUser).ThrowsForAnyArgs<BaseArgumentException>();
        RefreshTokenRepository.ExistsAsync(default, default, default).ReturnsForAnyArgs(false);

        var command = GenerateCommandWithValidDefaultData();

        var handler = CreateHandler();

        Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }

    private CreateAuthTokensCommandHandler CreateHandler() => new(RefreshTokenRepository, JwtAuthService);

    private CreateAuthTokensCommand GenerateCommandWithValidDefaultData(Guid? deviceId = null) => new(DefaultUser)
    {
        DeviceId = deviceId
    };
}