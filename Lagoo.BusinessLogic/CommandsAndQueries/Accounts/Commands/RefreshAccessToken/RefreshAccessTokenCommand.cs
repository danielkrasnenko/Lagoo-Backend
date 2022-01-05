using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Commands.RefreshAccessToken;

/// <summary>
///   Command for refreshing access token
/// </summary>
public class RefreshAccessTokenCommand : IRequest<RefreshAccessTokenResponseDto>
{
    /// <summary>
    ///   Expired access token that needs to be refreshed
    /// </summary>
    public string AccessToken { get; set; } = string.Empty;
    
    /// <summary>
    ///   Refresh token value for refreshing access token
    /// </summary>
    public string RefreshTokenValue { get; set; } = string.Empty;
}