using System.Security.Claims;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RefreshAccessToken;

/// <summary>
/// Request handler for <see cref="RefreshAccessTokenCommand"/>
/// </summary>
public class RefreshAccessTokenCommandHandler : IRequestHandler<RefreshAccessTokenCommand, RefreshAccessTokenResponseDto>
{
    private readonly IAppDbContext _context;

    private readonly IJwtAuthService _jwtAuthService;

    public RefreshAccessTokenCommandHandler(IAppDbContext context, IJwtAuthService jwtAuthService)
    {
        _context = context;
        _jwtAuthService = jwtAuthService;
    }

    public async Task<RefreshAccessTokenResponseDto> Handle(RefreshAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = ExtractUserIdFromAccessToken(request.AccessToken);

        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.Owner)
            .FirstOrDefaultAsync(rt => rt.Value == request.RefreshTokenValue && rt.OwnerId == userId, cancellationToken);

        if (refreshToken is null)
        {
            throw new BadRequestException("Refresh token was not found");
        }

        var (accessToken, accessTokenExpiresAt) = await _jwtAuthService.GenerateAccessTokenAsync(refreshToken.Owner!);

        return new RefreshAccessTokenResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpiresAt
        };
    }

    private Guid ExtractUserIdFromAccessToken(string accessToken)
    {
        var claimsPrincipal = _jwtAuthService.GetPrincipalFromToken(accessToken);
        var claimUserId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (claimUserId is null || !Guid.TryParse(claimUserId, out var userId))
        {
            throw new BadRequestException("Invalid JWT claims");
        }

        return userId;
    }
}