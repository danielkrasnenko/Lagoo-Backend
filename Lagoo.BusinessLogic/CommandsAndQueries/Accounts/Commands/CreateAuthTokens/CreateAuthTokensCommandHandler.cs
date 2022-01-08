using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Lagoo.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;

public class CreateAuthTokensCommandHandler : IRequestHandler<CreateAuthTokensCommand, AuthenticationDataDto>
{
    private readonly IAppDbContext _context;

    private readonly IJwtAuthService _jwtAuthService;

    public CreateAuthTokensCommandHandler(IAppDbContext context, IJwtAuthService jwtAuthService)
    {
        _context = context;
        _jwtAuthService = jwtAuthService;
    }

    public async Task<AuthenticationDataDto> Handle(CreateAuthTokensCommand request, CancellationToken cancellationToken)
    {
        var (accessToken, accessTokenExpirationDate) = await _jwtAuthService.GenerateAccessTokenAsync(request.User);
        var refreshToken = await GetRefreshTokenAsync(request.User, request.DeviceId, cancellationToken);

        await _context.SaveChangesAsync(CancellationToken.None);
        
        return new AuthenticationDataDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpirationDate,
            RefreshTokenValue = refreshToken.Value,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            DeviceId = refreshToken.DeviceId
        };
    }
    
    private async Task<RefreshToken> GetRefreshTokenAsync(AppUser user, Guid? deviceId, CancellationToken cancellationToken)
    {
        if (deviceId is null)
        {
            return CreateRefreshToken(user);
        }

        var refreshToken =
            await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.OwnerId == user.Id && rt.DeviceId == deviceId,
                cancellationToken);

        return refreshToken is null ? CreateRefreshToken(user) : _jwtAuthService.UpdateRefreshToken(refreshToken);
    }

    private RefreshToken CreateRefreshToken(AppUser user)
    {
        var newRefreshToken = _jwtAuthService.GenerateRefreshToken(user, Guid.NewGuid());
        _context.RefreshTokens.Add(newRefreshToken);
        
        return newRefreshToken;
    }
}