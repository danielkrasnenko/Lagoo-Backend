using Lagoo.BusinessLogic.Common.UserAccessor;

namespace Lagoo.BusinessLogic.Common.Extensions;

/// <summary>
///   Extension methods for <see cref="IUserAccessor"/>
/// </summary>
public static class UserAccessorExtensions
{
    public static string? RetrieveClaimValue(this IUserAccessor userAccessor, string claimType) =>
        userAccessor.ClaimsPrincipal?.FindFirst(c => c.Type == claimType)?.Value;

    public static List<string>? RetrieveClaimValues(this IUserAccessor userAccessor, string claimType) =>
        userAccessor.ClaimsPrincipal?.FindAll(c => c.Type == claimType).Select(c => c.Value).ToList();
}