using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Base;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Lagoo.Domain.Entities;
using Lagoo.Infrastructure.AppOptions.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Lagoo.Infrastructure.Services.JwtAuthService;

/// <summary>
///   Service for dealing with JWT in the app
/// </summary>
public class JwtAuthService : IJwtAuthService
{
    private readonly UserManager<AppUser> _userManager;
    
    private readonly JwtAuthOptions _authOptions;

    public JwtAuthService(UserManager<AppUser> userManager, IOptions<JwtAuthOptions> jwtAuthOptions)
    {
        _authOptions = jwtAuthOptions.Value;
        _userManager = userManager;
    }

    /// <summary>
    ///   Generates an access token for a user and his/her role
    /// </summary>
    /// <param name="user">The user to generate access token for</param>
    /// <param name="userRole">The user role</param>
    /// <returns>The Task that represents the asynchronous operation,
    ///  containing a Tuple of new encrypted access token and its expiration date</returns>
    /// <exception cref="BaseArgumentException">User does not have a role</exception>
    public async Task<(string, DateTime)> GenerateAccessTokenAsync(AppUser user, string? userRole = null)
    {
        userRole ??= (await _userManager.GetRolesAsync(user)).FirstOrDefault(); 

        if (userRole is null)
        {
            throw new BaseArgumentException($"{nameof(user)} has to have a role");
        }

        var claims = BuildUserClaims(user, userRole);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = SigningCredentials,
            Issuer = _authOptions.Issuer,
            Audience = _authOptions.Audience,
            Expires = DateTime.UtcNow.AddMinutes(_authOptions.AccessTokenExpirationInMin)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var newToken = tokenHandler.CreateToken(tokenDescriptor);
        var serializedToken = tokenHandler.WriteToken(newToken);
        
        return (serializedToken, newToken.ValidTo);
    }

    /// <summary>
    ///   Generates a refresh token for a user and device ID
    /// </summary>
    /// <param name="user">The user to generate access token for</param>
    /// <param name="deviceId">A device ID</param>
    /// <returns>New refresh token</returns>
    public RefreshToken GenerateRefreshToken(AppUser user, Guid deviceId) => new()
    {
        Value = GenerateRefreshTokenValue(),
        ExpiresAt = DateTime.UtcNow.AddMinutes(_authOptions.RefreshTokenExpirationInMin),
        Owner = user,
        OwnerId = user.Id,
        DeviceId = deviceId
    };

    /// <summary>
    ///   Generates an update refresh token DTO
    /// </summary>
    /// <returns>Update Refresh Token DTO</returns>
    public UpdateRefreshTokenDto GenerateDataForRefreshTokenUpdate()
    {
        var updateRefreshTokenDto = new UpdateRefreshTokenDto();
        updateRefreshTokenDto.Value = GenerateRefreshTokenValue();
        updateRefreshTokenDto.ExpiresAt = DateTime.UtcNow.AddMinutes(_authOptions.RefreshTokenExpirationInMin);
        updateRefreshTokenDto.LastModifiedAt = DateTime.UtcNow;

        return updateRefreshTokenDto;
    }

    /// <summary>
    ///   Gets a claims principal based on provided access token
    /// </summary>
    /// <param name="token">Access token to extract claims principal from</param>
    /// <returns>Claims principal of an access token</returns>
    /// <exception cref="BaseArgumentException">Access token is invalid</exception>
    public ClaimsPrincipal GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = GetSymmetricSecurityKey(_authOptions.Secret)
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, parameters, out var securityToken);
            ValidateSecurityToken(securityToken);

            return principal;
        }
        catch (Exception e)
        {
            throw new BaseArgumentException($"JWT validation failed: {e.Message}");
        }
    }

    /// <summary>
    ///   Gets a symmetric security key based on a secret
    /// </summary>
    /// <param name="secret">A secret for generating symmetric security key</param>
    /// <returns>A symmetric security key</returns>
    /// <exception cref="NullReferenceException">Provided secret is empty</exception>
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string secret)
    {
        return !string.IsNullOrWhiteSpace(secret)
            ? new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            : throw new NullReferenceException(nameof(secret));
    }

    private string GenerateRefreshTokenValue()
    {
        var randomNumber = new byte[30];

        using var randomNumberGenerator = RandomNumberGenerator.Create();

        randomNumberGenerator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    private SigningCredentials SigningCredentials => new (GetSymmetricSecurityKey(_authOptions.Secret), JwtAuthOptions.SecurityAlgorithm);

    private List<Claim> BuildUserClaims(AppUser user, string userRole) => new()
    {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.AuthTime, DateTimeOffset.UtcNow.ToString(CultureInfo.InvariantCulture)),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.GivenName, user.FirstName),
        new Claim(ClaimTypes.Surname, user.LastName),
        new Claim(ClaimTypes.Role, userRole)
    };

    private void ValidateSecurityToken(SecurityToken? securityToken)
    {
        if (securityToken is not JwtSecurityToken jwtSecurityToken
            || !jwtSecurityToken.Header.Alg.Equals(JwtAuthOptions.SecurityAlgorithm, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new BaseArgumentException("JWT validation failed");
        }
    }
}