using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;

public class CreateAuthTokensCommandHandler : IRequestHandler<CreateAuthTokensCommand, AuthenticationTokensDto>
{
    private readonly IAppDbContext _context;

    private readonly IJwtAuthService _jwtAuthService;

    private readonly IStringLocalizer<AccountResources> _accountLocalizer;

    public CreateAuthTokensCommandHandler(IAppDbContext context, IJwtAuthService jwtAuthService, IStringLocalizer<AccountResources> accountLocalizer)
    {
        _context = context;
        _jwtAuthService = jwtAuthService;
        _accountLocalizer = accountLocalizer;
    }

    public async Task<AuthenticationTokensDto> Handle(CreateAuthTokensCommand request, CancellationToken cancellationToken)
    {
        var (accessToken, accessTokenExpirationDate) = await _jwtAuthService.GenerateAccessTokenAsync(request.User);
        var refreshToken = await GetRefreshTokenAsync(request.User, request.DeviceId, cancellationToken);

        await _context.SaveChangesAsync(CancellationToken.None);
        
        return new AuthenticationTokensDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpirationDate,
            RefreshTokenValue = refreshToken.Value,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }
    
    private async Task<RefreshToken> GetRefreshTokenAsync(AppUser user, Guid deviceId, CancellationToken cancellationToken)
    {
        var refreshToken =
            await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.OwnerId == user.Id && rt.DeviceId == deviceId,
                cancellationToken);

        if (refreshToken is not null)
            return _jwtAuthService.UpdateRefreshToken(refreshToken);

        var newRefreshToken = _jwtAuthService.GenerateRefreshToken(user.Id, deviceId);
        _context.RefreshTokens.Add(newRefreshToken);

        return newRefreshToken;
    }
}