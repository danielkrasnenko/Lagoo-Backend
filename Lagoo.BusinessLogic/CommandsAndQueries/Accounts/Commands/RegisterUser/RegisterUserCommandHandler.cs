using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;
using Lagoo.BusinessLogic.Common.Services.JwtAuthService;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using Lagoo.Domain.Types;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RegisterUser;

/// <summary>
/// Request handler for <see cref="RegisterUserCommand"/>
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponseDto>
{
    private readonly UserManager<AppUser> _userManager;

    private readonly IJwtAuthService _jwtAuthService;

    private readonly IExternalAuthServicesManager _externalAuthServicesManager;

    private readonly IStringLocalizer<AccountResources> _accountLocalizer;

    public RegisterUserCommandHandler(UserManager<AppUser> userManager, IJwtAuthService jwtAuthService, IExternalAuthServicesManager externalAuthServicesManager, IStringLocalizer<AccountResources> accountLocalizer)
    {
        _userManager = userManager;
        _jwtAuthService = jwtAuthService;
        _externalAuthServicesManager = externalAuthServicesManager;
        _accountLocalizer = accountLocalizer;
    }

    public async Task<RegisterUserResponseDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        if (request.ExternalAuthService is not null && request.AccessToken is not null)
        {
            await CreateAccountWithExternalLogin(user, request.ExternalAuthService.Value, request.AccessToken);
        }
        else if (request.Password is not null)
        {
            await CreateAccount(user, request.Password);
        }
        else
        {
            throw new BadRequestException(_accountLocalizer["InvalidData"]);
        }

        var (accessToken, accessTokenExpirationDate) = await _jwtAuthService.GenerateAccessTokenAsync(user, AppUserRole.User);
        var refreshToken = _jwtAuthService.GenerateRefreshToken(user.Id);

        return new RegisterUserResponseDto
        {
            AccessToken = accessToken,
            AccessTokenExpiresAt = accessTokenExpirationDate,
            RefreshTokenValue = refreshToken.Value,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }

    private async Task CreateAccount(AppUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new BadRequestException(result);
        }
    }

    private async Task CreateAccountWithExternalLogin(AppUser user, ExternalAuthService externalAuthService,
        string accessToken)
    {
        var result = await _userManager.CreateAsync(user);
        
        if (!result.Succeeded)
        {
            throw new BadRequestException(result);
        }
        
        var loginResult = await _externalAuthServicesManager.BindUserAsync(user, externalAuthService, accessToken);
            
        if (!loginResult.Succeeded)
        {
            throw new BadRequestException(loginResult);
        }
    }
}