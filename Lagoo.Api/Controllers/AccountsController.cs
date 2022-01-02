using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUserViaExternalService;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RefreshAccessToken;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RegisterUser;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lagoo.Api.Controllers;

[Route("api/accounts")]
public class AccountsController : ApiController
{
    public AccountsController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Register a user in the app, optionally via an external authentication
    /// </summary>
    /// <param name="command">User data for registration</param>
    /// <returns>Access and Refresh tokens, and their expiration dates</returns>
    [HttpPost("auth/register")]
    public Task<AuthenticationTokensDto> RegisterUser([FromBody] RegisterUserCommand command) => Mediator.Send(command);

    /// <summary>
    /// Login a user in the app
    /// </summary>
    /// <param name="command">User data to login in the app</param>
    /// <returns>Access and Refresh tokens, and their expiration dates</returns>
    [HttpPost("auth/login")]
    public Task<AuthenticationTokensDto> LoginUser([FromBody] LoginUserCommand command) => Mediator.Send(command);

    /// <summary>
    /// Login a user via any supported external authentication service
    /// </summary>
    /// <param name="command">External authentication service, its access token and an optional refresh token if exists on a device</param>
    /// <returns>Access and Refresh tokens, and their expiration dates</returns>
    [HttpPost("auth/login/external-service")]
    public Task<AuthenticationTokensDto> LoginUserViaFacebook([FromBody] LoginUserViaExternalServiceCommand command) =>
        Mediator.Send(command);

    /// <summary>
    /// Refresh access token using refresh token for further access to guarded endpoints
    /// </summary>
    /// <param name="command">Expired Access token and Refresh token</param>
    /// <returns>New Access token</returns>
    [HttpPost("auth/refresh")]
    public Task<RefreshAccessTokenResponseDto> RefreshAccessToken([FromBody] RefreshAccessTokenCommand command) =>
        Mediator.Send(command);
}