using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.Domain.Entities;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;

/// <summary>
/// Command for creating authentication tokens to send to the frontend for further authorization
/// </summary>
public class CreateAuthTokensCommand : IRequest<AuthenticationTokensDto>
{
    public CreateAuthTokensCommand(AppUser user)
    {
        User = user;
    }

    /// <summary>
    /// User account which to create authentication tokens for
    /// </summary>
    public AppUser User { get; set; }
    
    /// <summary>
    /// Refresh token value for updating Refresh token if it exists on a device,
    /// otherwise create a new one
    /// </summary>
    public string? RefreshTokenValue { get; set; } = string.Empty;
}