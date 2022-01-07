using Lagoo.Domain.Enums;
using MediatR;

namespace Lagoo.BusinessLogic.CommandsAndQueries.Accounts.Queries.GetExternalAuthServiceUserInfo;

/// <summary>
///   A Query for getting user info from specified <see cref="ExternalAuthService"/>
/// </summary>
public class GetExternalAuthServiceUserInfoQuery : IRequest<GetExternalAuthServiceUserInfoResponseDto>
{
    /// <summary>
    ///   External authentication service
    /// </summary>
    public ExternalAuthService ExternalAuthService { get; set; }

    /// <summary>
    ///   Access token to external authentication service
    /// </summary>
    public string ExternalAuthServiceAccessToken { get; set; } = string.Empty;
}