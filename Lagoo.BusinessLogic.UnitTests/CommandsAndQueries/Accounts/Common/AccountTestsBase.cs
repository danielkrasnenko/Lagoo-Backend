using System;
using System.Security.Claims;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.Domain.Entities;
using Lagoo.Infrastructure.Services.FacebookAuthService;
using NSubstitute;
using NUnit.Framework;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;

/// <summary>
///   Base class for account tests
/// </summary>
public class AccountTestsBase : TestsBase
{
    protected const string ValidPassword = "=#IoP123IoP#=";
    
    protected const string ExternalAuthServiceDefaultUserId = "user_unique_identifier";

    protected const string ExternalAuthServiceAccessToken = "access_token";
    
    protected const string DefaultAccessToken = "access_token_value";
    
    protected static readonly DateTime DefaultAccessTokenExpirationDate = DateTime.UtcNow.AddDays(1);

    protected static readonly ClaimsPrincipal DefaultClaimsPrincipal =
        new(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, DefaultUserId.ToString()) }));
    
    protected static readonly Guid DefaultDeviceId = Guid.NewGuid();
    
    protected static readonly Guid NewDeviceId = Guid.NewGuid();
    
    protected const long DefaultRefreshTokenId = 1;

    protected const string DefaultRefreshTokenValue = "very very long hash";

    protected static readonly DateTime DefaultRefreshTokenExpirationDate = DateTime.UtcNow.AddDays(1);

    protected static readonly RefreshToken DefaultRefreshToken = GenerateDefaultRefreshToken(DefaultDeviceId, DefaultRefreshTokenExpirationDate, null);

    protected static readonly ReadRefreshTokenDto DefaultReadRefreshTokenDto = new ReadRefreshTokenDto
    {
        Id = DefaultRefreshTokenId,
        Value = DefaultRefreshTokenValue,
        DeviceId = DefaultDeviceId,
        ExpiresAt = DefaultRefreshTokenExpirationDate,
        OwnerId = DefaultUserId
    };

    protected static readonly UpdateRefreshTokenDto DefaultUpdateRefreshTokenDto = new()
    {
        Value = DefaultRefreshTokenValue,
        ExpiresAt = DateTime.MaxValue,
        LastModifiedAt = DateTime.UtcNow
    };

    protected static readonly RefreshToken DefaultNewRefreshToken = GenerateDefaultRefreshToken(NewDeviceId, DefaultRefreshTokenExpirationDate, null);

    protected IJwtAuthService JwtAuthService = CreateJwtAuthServiceSubstitution();

    public AccountTestsBase()
    {
        ExternalAuthServicesManager.GetUserInfoAsync(default, "").ReturnsForAnyArgs(new FacebookUserInfo
        {
            Id = ExternalAuthServiceDefaultUserId,
            Email = DefaultUserEmail,
            FirstName = DefaultUser.FirstName,
            LastName = DefaultUser.LastName
        });
    }
    
    protected AuthenticationDataDto GenerateDefaultAuthDataDto(Guid deviceId) => new()
    {
        AccessToken = DefaultAccessToken,
        AccessTokenExpiresAt = DefaultAccessTokenExpirationDate,
        RefreshTokenValue = DefaultRefreshTokenValue,
        RefreshTokenExpiresAt = DefaultRefreshToken.ExpiresAt,
        DeviceId = deviceId
    };
    
    protected void AssertAuthenticationDataDtoContainsDefaultData(AuthenticationDataDto result)
    {
        Assert.AreEqual(DefaultAccessToken, result.AccessToken);
        Assert.AreEqual(DefaultAccessTokenExpirationDate, result.AccessTokenExpiresAt);
        Assert.AreEqual(DefaultRefreshTokenValue, result.RefreshTokenValue);
        Assert.AreEqual(DefaultRefreshTokenExpirationDate, result.RefreshTokenExpiresAt);   
    }

    protected static RefreshToken GenerateDefaultRefreshToken(Guid deviceId, DateTime expiresAt, DateTime? lastModifiedAt) => new()
    {
        Id = DefaultRefreshTokenId,
        Value = DefaultRefreshTokenValue,
        DeviceId = deviceId,
        ExpiresAt = expiresAt,
        LastModifiedAt = lastModifiedAt,
        OwnerId = DefaultUserId,
        Owner = DefaultUser
    };

    private static IJwtAuthService CreateJwtAuthServiceSubstitution()
    {
        var jwtAuthService = Substitute.For<IJwtAuthService>();

        jwtAuthService.GenerateAccessTokenAsync(new AppUser()).ReturnsForAnyArgs((DefaultAccessToken, DefaultAccessTokenExpirationDate));

        jwtAuthService.GenerateRefreshToken(new AppUser(), default).ReturnsForAnyArgs(DefaultRefreshToken);

        jwtAuthService.GenerateDataForRefreshTokenUpdate().ReturnsForAnyArgs(DefaultUpdateRefreshTokenDto);

        jwtAuthService.GetPrincipalFromToken("").ReturnsForAnyArgs(DefaultClaimsPrincipal);

        return jwtAuthService;
    }
}