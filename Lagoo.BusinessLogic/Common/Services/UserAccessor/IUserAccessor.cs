using System.Security.Claims;

namespace Lagoo.BusinessLogic.Common.Services.UserAccessor;

/// <summary>
///   An interface for declaring needed functionality to work with JWT
/// </summary>
public interface IUserAccessor
{
    public Guid? UserId { get; }

    public string? Role { get; }

    public bool IsAuthenticated { get; }

    public ClaimsPrincipal? ClaimsPrincipal { get; }
    
}