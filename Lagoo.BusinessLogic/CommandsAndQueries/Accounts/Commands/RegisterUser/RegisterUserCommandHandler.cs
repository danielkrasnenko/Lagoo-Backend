using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using Lagoo.Domain.Enums;
using Lagoo.Domain.Types;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RegisterUser;

/// <summary>
///   Request handler for <see cref="RegisterUserCommand"/>
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthenticationDataDto>
{
    private readonly IUserRepository _userRepository;

    private readonly IMediator _mediator;

    private readonly IExternalAuthServicesManager _externalAuthServicesManager;

    public RegisterUserCommandHandler(IUserRepository userRepository, IMediator mediator,
        IExternalAuthServicesManager externalAuthServicesManager)
    {
        _mediator = mediator;
        _externalAuthServicesManager = externalAuthServicesManager;
        _userRepository = userRepository;
    }

    public async Task<AuthenticationDataDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new AppUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        if (request.ExternalAuthService is not null && request.ExternalAuthServiceAccessToken is not null)
        {
            await CreateAccountWithExternalLoginAsync(user, request.ExternalAuthService.Value, request.ExternalAuthServiceAccessToken);
        }
        else if (request.Password is not null)
        {
            await CreateAccountAsync(user, request.Password);
        }
        else
        {
            throw new BadRequestException(AccountResources.InvalidData);
        }

        await AddDefaultRoleToUserAsync(user);

        return await _mediator.Send(new CreateAuthTokensCommand(user) { DeviceId = request.DeviceId }, cancellationToken);
    }

    private async Task CreateAccountAsync(AppUser user, string password)
    {
        var result = await _userRepository.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new BadRequestException(result);
        }
    }

    private async Task CreateAccountWithExternalLoginAsync(AppUser user, ExternalAuthService externalAuthService,
        string accessToken)
    {
        var result = await _userRepository.CreateAsync(user);
        
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

    private async Task AddDefaultRoleToUserAsync(AppUser user)
    {
        var result = await _userRepository.AddToRoleAsync(user, AppUserRole.User);

        if (!result.Succeeded)
        {
            throw new BadRequestException(result);
        }
    }
}