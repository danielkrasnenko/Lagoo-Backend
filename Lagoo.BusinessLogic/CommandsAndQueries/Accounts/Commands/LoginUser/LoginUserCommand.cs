using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUser;

/// <summary>
/// Command to login user in the app via his/her credentials of a local account
/// </summary>
public class LoginUserCommand : IRequest<AuthenticationTokensDto>
{
    /// <summary>
    /// User email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token value for updating Refresh token if it exists on a device,
    /// otherwise create a new one
    /// </summary>
    public string? RefreshTokenValue { get; set; } = string.Empty;
}