using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.Exceptions.Base;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.Domain.Entities;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;

public class CreateAuthTokensCommandHandler : IRequestHandler<CreateAuthTokensCommand, AuthenticationDataDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    private readonly IJwtAuthService _jwtAuthService;

    public CreateAuthTokensCommandHandler(IRefreshTokenRepository refreshTokenRepository, IJwtAuthService jwtAuthService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _jwtAuthService = jwtAuthService;
    }

    public async Task<AuthenticationDataDto> Handle(CreateAuthTokensCommand request, CancellationToken cancellationToken)
    {
        var (accessToken, accessTokenExpirationDate) = await TryCreateAccessTokenAsync(request.User);
        var refreshToken = await GetRefreshTokenAsync(request.User, request.DeviceId, cancellationToken);

        return new AuthenticationDataDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpirationDate,
            RefreshTokenValue = refreshToken.Value,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
            DeviceId = refreshToken.DeviceId
        };
    }

    private Task<(string, DateTime)> TryCreateAccessTokenAsync(AppUser user)
    {
        try
        {
            return _jwtAuthService.GenerateAccessTokenAsync(user);
        }
        catch (BaseArgumentException exception)
        {
            throw new BadRequestException(exception.Message);
        }
    }
    
    private async Task<ReadRefreshTokenDto> GetRefreshTokenAsync(AppUser user, Guid? deviceId, CancellationToken cancellationToken)
    {
        if (deviceId is null)
        {
            return await CreateRefreshTokenAsync(user);
        }

        var refreshTokenExists = await _refreshTokenRepository.ExistsAsync(user.Id, deviceId.Value, cancellationToken);

        if (!refreshTokenExists)
        {
            return await CreateRefreshTokenAsync(user);
        }
        
        var dto = _jwtAuthService.GenerateDataForRefreshTokenUpdate();
        return (await _refreshTokenRepository.UpdateAsync(user.Id, deviceId.Value, dto, cancellationToken))!;
    }

    private async Task<ReadRefreshTokenDto> CreateRefreshTokenAsync(AppUser user)
    {
        var newRefreshToken = _jwtAuthService.GenerateRefreshToken(user, Guid.NewGuid());
        
        return await _refreshTokenRepository.SaveAsync(newRefreshToken);
    }
}