using System.Security.Claims;
using Lagoo.Domain.Entities;

namespace Lagoo.BusinessLogic.Common.Services.JwtAuthService;

/// <summary>
///   An interface for declaring needed functionality to work with JWT Authentication and Authorization
/// </summary>
public interface IJwtAuthService
{
    public Task<(string, DateTime)> GenerateAccessTokenAsync(AppUser user, string? userRole = null);

    public RefreshToken GenerateRefreshToken(AppUser user, Guid deviceId);

    public RefreshToken UpdateRefreshToken(RefreshToken refreshToken);

    public ClaimsPrincipal GetPrincipalFromToken(string token);
}