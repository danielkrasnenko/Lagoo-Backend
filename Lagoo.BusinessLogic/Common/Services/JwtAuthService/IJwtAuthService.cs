using System.Security.Claims;
using Lagoo.Domain.Entities;

namespace Lagoo.BusinessLogic.Common.Services.JwtAuthService;

public interface IJwtAuthService
{
    public Task<string> GenerateAccessToken(AppUser user);

    public RefreshToken GenerateRefreshToken(Guid ownerId);

    public RefreshToken UpdateRefreshToken(RefreshToken refreshToken);

    public ClaimsPrincipal GetPrincipalFromToken(string token);
}