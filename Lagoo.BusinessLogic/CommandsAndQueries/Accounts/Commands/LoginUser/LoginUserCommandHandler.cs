using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.ExternalServices.Database;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginUserResponseDto>
{
    private readonly IAppDbContext _context;

    private readonly UserManager<AppUser> _userManager;

    private readonly IJwtAuthService _jwtAuthService;

    private readonly IStringLocalizer<AccountResources> _accountLocalizer;

    public LoginUserCommandHandler(IAppDbContext context, UserManager<AppUser> userManager, IJwtAuthService jwtAuthService, IStringLocalizer<AccountResources> accountLocalizer)
    {
        _context = context;
        _userManager = userManager;
        _jwtAuthService = jwtAuthService;
        _accountLocalizer = accountLocalizer;
    }
    
    public async Task<LoginUserResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user is null)
        {
            throw new BadRequestException(_accountLocalizer["AccountWasNotFoundByEmail"]);
        }
        
        await ValidateUserDataAsync(user, request.Password);
        
        var (accessToken, accessTokenExpirationDate) = await _jwtAuthService.GenerateAccessTokenAsync(user);
        var refreshToken = await GetRefreshTokenAsync(user.Id, request.RefreshTokenValue, cancellationToken);

        await _context.SaveChangesAsync(CancellationToken.None);
        
        return new LoginUserResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpirationDate,
            RefreshTokenValue = refreshToken.Value,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }

    private async Task ValidateUserDataAsync(AppUser user, string password)
    {
        var givenPasswordIsValid = await _userManager.CheckPasswordAsync(user, password);
        
        if (!givenPasswordIsValid)
        {
            throw new BadRequestException(_accountLocalizer["InvalidPassword"]);
        }
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