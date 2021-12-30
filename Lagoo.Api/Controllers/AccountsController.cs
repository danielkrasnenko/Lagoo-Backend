using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;
using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lagoo.Api.Controllers;

[Route("api/accounts")]
public class AccountsController : ApiController
{
    public AccountsController(IMediator mediator) : base(mediator) { }

    /// <summary>
    /// Register a user in the app optionally via an external authentication
    /// </summary>
    /// <param name="command">User data for registration</param>
    /// <returns>Access and Refresh tokens, and their expiration dates</returns>
    [HttpPost("auth/register")]
    public Task<RegisterUserResponseDto> RegisterUser([FromBody] RegisterUserCommand command) => Mediator.Send(command);

    /// <summary>
    /// Login a user in the app
    /// </summary>
    /// <param name="command">User data to login in the app</param>
    /// <returns>Access and Refresh tokens, and their expiration dates</returns>
    [HttpPost("auth/login")]
    public Task<LoginUserResponseDto> LoginUser([FromBody] LoginUserCommand command) => Mediator.Send(command);
}