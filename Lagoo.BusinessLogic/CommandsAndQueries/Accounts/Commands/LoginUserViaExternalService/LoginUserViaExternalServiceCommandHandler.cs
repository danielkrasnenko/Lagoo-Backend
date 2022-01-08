using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUserViaExternalService;

/// <summary>
///   Request handler for <see cref="LoginUserViaExternalServiceCommand"/>
/// </summary>
public class LoginUserViaExternalServiceCommandHandler : IRequestHandler<LoginUserViaExternalServiceCommand, AuthenticationDataDto>
{
    private readonly IExternalAuthServicesManager _externalAuthServicesManager;

    private readonly UserManager<AppUser> _userManager;

    private readonly IMediator _mediator;

    private readonly IStringLocalizer<AccountResources> _accountLocalizer;

    public LoginUserViaExternalServiceCommandHandler(IExternalAuthServicesManager externalAuthServicesManager, UserManager<AppUser> userManager, IMediator mediator, IStringLocalizer<AccountResources> accountLocalizer)
    {
        _externalAuthServicesManager = externalAuthServicesManager;
        _userManager = userManager;
        _mediator = mediator;
        _accountLocalizer = accountLocalizer;
    }

    public async Task<AuthenticationDataDto> Handle(LoginUserViaExternalServiceCommand request, CancellationToken cancellationToken)
    {
        var userInfo =
            await _externalAuthServicesManager.GetUserInfoAsync(request.ExternalAuthService,
                request.ExternalServiceAccessToken);

        var user = await _userManager.FindByLoginAsync(request.ExternalAuthService.GetEnumDescription(), userInfo.Id);

        if (user is null)
        {
            throw new BadRequestException(_accountLocalizer["AccountWasNotFoundByExternalAuthServiceData"]);
        }
        
        return await _mediator.Send(new CreateAuthTokensCommand(user) { DeviceId = request.DeviceId }, cancellationToken);
    }
}