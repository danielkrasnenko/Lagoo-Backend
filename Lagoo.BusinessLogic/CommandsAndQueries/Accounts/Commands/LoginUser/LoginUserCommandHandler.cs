using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;

/// <summary>
/// Request handler for <see cref="LoginUserCommand"/>
/// </summary>
public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthenticationTokensDto>
{
    private readonly UserManager<AppUser> _userManager;

    private readonly IMediator _mediator;

    private readonly IStringLocalizer<AccountResources> _accountLocalizer;

    public LoginUserCommandHandler(UserManager<AppUser> userManager, IMediator mediator, IStringLocalizer<AccountResources> accountLocalizer)
    {
        _userManager = userManager;
        _mediator = mediator;
        _accountLocalizer = accountLocalizer;
    }
    
    public async Task<AuthenticationTokensDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new BadRequestException(_accountLocalizer["AccountWasNotFoundByEmail"]);
        }
        
        await ValidateUserDataAsync(user, request.Password);
        
        return await _mediator.Send(new CreateAuthTokensCommand(user) { RefreshTokenValue = request.RefreshTokenValue },
            cancellationToken);
    }

    private async Task ValidateUserDataAsync(AppUser user, string password)
    {
        var givenPasswordIsValid = await _userManager.CheckPasswordAsync(user, password);
        
        if (!givenPasswordIsValid)
        {
            throw new BadRequestException(_accountLocalizer["InvalidPassword"]);
        }
    }
}