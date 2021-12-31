using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
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
        var refreshToken = await GetRefreshTokenAsync(request.User.Id, request.RefreshTokenValue, cancellationToken);

        await _context.SaveChangesAsync(CancellationToken.None);
        
        return new AuthenticationTokensDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpirationDate,
            RefreshTokenValue = refreshToken.Value,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }
    
    private async Task<RefreshToken> GetRefreshTokenAsync(Guid userId, string? refreshTokenValue, CancellationToken cancellationToken)
    {
        if (refreshTokenValue is null)
        {
            var refreshToken = _jwtAuthService.GenerateRefreshToken(userId);
            _context.RefreshTokens.Add(refreshToken);

            return refreshToken;
        }
        
        var outdatedRefreshToken =
            await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Value == refreshTokenValue,
                cancellationToken);

        if (outdatedRefreshToken is null)
        {
            throw new BadRequestException(_accountLocalizer["RefreshTokenWasNotFound"]);
        }

        return _jwtAuthService.UpdateRefreshToken(outdatedRefreshToken);
    }
}