using System;
using Lagoo.BusinessLogic.Common.ExternalServices.FacebookAuthService;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Lagoo.BusinessLogic.UnitTests.Common.Base;
using Lagoo.Domain.Entities;
using NSubstitute;

namespace Lagoo.BusinessLogic.UnitTests.CommandsAndQueries.Accounts.Common;

/// <summary>
///   Base class for account tests
/// </summary>
public class AccountTestsBase : TestsBase
{
    protected const string ValidPassword = "=#IoP123IoP#=";
    
    protected const string ExternalAuthServiceDefaultUserId = "user_unique_identifier";

    protected const string ExternalAuthServiceAccessToken = "access_token";
    
    protected const string DefaultAccessTokenValue = "access_token_value";
    
    protected static readonly DateTime DefaultAccessTokenExpirationDate = DateTime.UtcNow;
    
    protected static readonly Guid DefaultDeviceId = Guid.NewGuid();
    
    protected static readonly Guid NewDeviceId = Guid.NewGuid();
    
    protected const long DefaultRefreshTokenId = 1;

    protected const string DefaultRefreshTokenValue = "very very long hash";

    protected static readonly RefreshToken DefaultRefreshToken = GenerateDefaultRefreshToken(DateTime.UtcNow, null);

    protected static readonly RefreshToken UpdatedDefaultRefreshToken = GenerateDefaultRefreshToken(DateTime.MaxValue, DateTime.UtcNow);

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

    protected static RefreshToken GenerateDefaultRefreshToken(DateTime expiresAt, DateTime? lastModifiedAt) => new()
    {
        Id = DefaultRefreshTokenId,
        Value = DefaultRefreshTokenValue,
        DeviceId = DefaultDeviceId,
        ExpiresAt = expiresAt,
        LastModifiedAt = lastModifiedAt,
        OwnerId = DefaultUserId,
        Owner = DefaultUser
    };

    private static IJwtAuthService CreateJwtAuthServiceSubstitution()
    {
        var jwtAuthService = Substitute.For<IJwtAuthService>();

        jwtAuthService.GenerateAccessTokenAsync(new AppUser()).ReturnsForAnyArgs((DefaultAccessTokenValue, DefaultAccessTokenExpirationDate));

        jwtAuthService.GenerateRefreshToken(new AppUser(), default).ReturnsForAnyArgs(DefaultRefreshToken);

        jwtAuthService.UpdateRefreshToken(new RefreshToken()).ReturnsForAnyArgs(UpdatedDefaultRefreshToken);

        return jwtAuthService;
    }
}