using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.BusinessLogic.Common.Exceptions.Api;
using Lagoo.BusinessLogic.Core.Repositories;
using Lagoo.BusinessLogic.Resources.CommandsAndQueries;
using Lagoo.Domain.Entities;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;

/// <summary>
///   Request handler for <see cref="LoginUserCommand"/>
/// </summary>
public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, AuthenticationDataDto>
{
    private readonly IUserRepository _userRepository;

    private readonly IMediator _mediator;

    public LoginUserCommandHandler(IUserRepository userRepository, IMediator mediator)
    {
        _userRepository = userRepository;
        _mediator = mediator;
    }
    
    public async Task<AuthenticationDataDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByEmailAsync(request.Email);

        if (user is null)
        {
            throw new BadRequestException(AccountResources.AccountWasNotFoundByEmail);
        }
        
        await ValidateUserDataAsync(user, request.Password);
        
        return await _mediator.Send(new CreateAuthTokensCommand(user) { DeviceId = request.DeviceId },
            cancellationToken);
    }

    private async Task ValidateUserDataAsync(AppUser user, string password)
    {
        var givenPasswordIsValid = await _userRepository.CheckPasswordAsync(user, password);
        
        if (!givenPasswordIsValid)
        {
            throw new BadRequestException(AccountResources.InvalidPassword);
        }
    }
}