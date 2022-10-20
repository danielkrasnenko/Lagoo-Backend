using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Common.Extensions;
using Lagoo.BusinessLogic.Common.Services.ExternalAuthServicesManager;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUserViaExternalService;

/// <summary>
///   Request handler for <see cref="LoginUserViaExternalServiceCommand"/>
/// </summary>
public class LoginUserViaExternalServiceCommandHandler : IRequestHandler<LoginUserViaExternalServiceCommand, AuthenticationDataDto>
{
    private readonly IUserRepository _userRepository;
    
    private readonly IExternalAuthServicesManager _externalAuthServicesManager;

    private readonly IMediator _mediator;
    
    public LoginUserViaExternalServiceCommandHandler(IUserRepository userRepository,
        IExternalAuthServicesManager externalAuthServicesManager, IMediator mediator)
    {
        _userRepository = userRepository;
        _externalAuthServicesManager = externalAuthServicesManager;
        _mediator = mediator;
    }

    public async Task<AuthenticationDataDto> Handle(LoginUserViaExternalServiceCommand request, CancellationToken cancellationToken)
    {
        var userInfo = await _externalAuthServicesManager.GetUserInfoAsync(
            request.ExternalAuthService, request.ExternalAuthServiceAccessToken);

        var user = await _userRepository.FindByLoginAsync(request.ExternalAuthService.GetEnumDescription(), userInfo.Id);

        if (user is null)
        {
            throw new BadRequestException(AccountResources.AccountWasNotFoundByExternalAuthServiceData);
        }
        
        return await _mediator.Send(new CreateAuthTokensCommand(user) { DeviceId = request.DeviceId }, cancellationToken);
    }
}