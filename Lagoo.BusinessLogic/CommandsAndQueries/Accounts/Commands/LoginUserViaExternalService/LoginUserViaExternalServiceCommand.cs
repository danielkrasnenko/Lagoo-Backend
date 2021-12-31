using Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Common.Dtos;
using Lagoo.Domain.Enums;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.LoginUserViaExternalService;

/// <summary>
/// Command to login user in the app via an external authentication service
/// </summary>
public class LoginUserViaExternalServiceCommand : IRequest<AuthenticationTokensDto>
{
    public ExternalAuthService ExternalAuthService { get; set; }

    public string ExternalServiceAccessToken { get; set; } = string.Empty;
    
    public string? RefreshTokenValue { get; set; }
}