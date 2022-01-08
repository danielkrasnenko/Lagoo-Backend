using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.Domain.Entities;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.CreateAuthTokens;

/// <summary>
///   Command for creating authentication tokens to send to the frontend for further authorization
/// </summary>
public class CreateAuthTokensCommand : IRequest<AuthenticationDataDto>
{
    public CreateAuthTokensCommand(AppUser user)
    {
        User = user;
    }

    /// <summary>
    ///   User account for which to create authentication tokens
    /// </summary>
    public AppUser User { get; set; }
    
    /// <summary>
    ///   A Guid associated with a particular device after register or login.
    ///   Used for updating Refresh token if it existed on a device, otherwise create a new one
    /// </summary>
    public Guid? DeviceId { get; set; }
}