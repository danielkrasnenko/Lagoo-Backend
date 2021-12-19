using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Lagoo.BusinessLogic.Common.AppOptions.Services;
using Lagoo.BusinessLogic.Common.Exceptions.Base;
using Lagoo.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Lagoo.BusinessLogic.Common.Services.JwtAuthService;

/// <summary>
///  Service for managing JWT in the app
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

    public async Task<string> GenerateAccessToken(AppUser user, string? userRole = null)
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
        
        return tokenHandler.WriteToken(newToken);
    }

    public RefreshToken GenerateRefreshToken(Guid ownerId) => new()
    {
        Value = GenerateRefreshTokenValue(),
        ExpirationUtcDate = DateTimeOffset.UtcNow.AddMinutes(_authOptions.RefreshTokenExpirationInMin),
        OwnerId = ownerId
    };

    public RefreshToken UpdateRefreshToken(RefreshToken refreshToken)
    {
        refreshToken.Value = GenerateRefreshTokenValue();
        refreshToken.ExpirationUtcDate = DateTimeOffset.UtcNow.AddMinutes(_authOptions.RefreshTokenExpirationInMin);
        refreshToken.LastModifiedUtcDate = DateTimeOffset.UtcNow;

        return refreshToken;
    }

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